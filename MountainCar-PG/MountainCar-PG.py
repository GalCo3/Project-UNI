import numpy as np
import gymnasium as gym
import matplotlib.pyplot as plt
import tensorflow.compat.v1 as tf

def get_env(render_mode=None) -> gym.Env:
    args = {"id": 'MountainCar-v0',
            "render_mode": render_mode}
    return gym.make(**args)


# optimized for Tf2
tf.disable_v2_behavior()
print("tf_ver:{}".format(tf.__version__))

env = get_env()
np.random.seed(1)

class PolicyNetwork:
    def __init__(self, state_size, action_size, learning_rate, name='policy_network'):
        self.global_step = tf.Variable(0, trainable=False, name='global_step')
        with tf.variable_scope(name):
            self.state = tf.placeholder(tf.float32, [None, state_size], name="state")
            self.action = tf.placeholder(tf.int32, [action_size], name="action")
            self.td_error = tf.placeholder(tf.float32, name="td_error")

            tf2_initializer = tf.keras.initializers.glorot_normal(seed=0)

            
            self.W1 = tf.get_variable("W1", [state_size, action_size], initializer=tf2_initializer)
            self.b1 = tf.get_variable("b1", [action_size], initializer=tf2_initializer)
            self.output = tf.add(tf.matmul(self.state, self.W1), self.b1)
            

            self.actions_distribution = tf.squeeze(tf.nn.softmax(self.output))
            self.neg_log_prob = tf.nn.softmax_cross_entropy_with_logits_v2(logits=self.output, labels=self.action)
            self.loss = tf.reduce_mean(self.neg_log_prob *  self.td_error)

            self.learning_rate = tf.train.exponential_decay(learning_rate, self.global_step, decay_steps=1000,
                                                            decay_rate=0.999, staircase=True)
            self.learning_rate = tf.maximum(self.learning_rate, 0.0001)

                                                                                              #  global_step=self.global_step)
            self.optimizer = tf.train.AdamOptimizer(learning_rate=learning_rate).minimize(self.loss, global_step=self.global_step)

class ValueNetwork:
    def __init__(self, state_size, learning_rate, name='value_network'):
        with tf.variable_scope(name):
            self.state = tf.placeholder(tf.float32, [None, state_size], name="state")
            self.target = tf.placeholder(tf.float32, [None, 1], name="target")

            tf2_initializer = tf.keras.initializers.glorot_normal(seed=0)
            
            self.W1 = tf.get_variable("W1", [state_size, 1], initializer=tf2_initializer)
            self.b1 = tf.get_variable("b1", [1], initializer=tf2_initializer)
            self.value_estimate = tf.add(tf.matmul(self.state, self.W1), self.b1)
           

            self.loss = tf.losses.mean_squared_error(self.value_estimate, self.target)
            self.optimizer = tf.train.AdamOptimizer(learning_rate=learning_rate).minimize(self.loss)
GRID_SIZE = (25, 25)

location_low, velocity_low = env.observation_space.low
location_high, velocity_high = env.observation_space.high
lims = [(location_low, location_high), (velocity_low, velocity_high)]
location_low, location_high, velocity_low, velocity_high
def tile_coder_func(sample, dim_ranges=lims, grid_size=GRID_SIZE):

    grid = np.zeros(grid_size)
    indices = []
    for i, (dim_min, dim_max) in enumerate(dim_ranges):
        # Calculate the interval width for each dimension
        interval_width = (dim_max - dim_min) / grid_size[i]
        # Determine the index of the interval where the sample falls into
        index = int((sample[i] - dim_min) / interval_width)
        # Ensure the index is within bounds
        index = min(max(0, index), grid_size[i] - 1)
        indices.append(index)
    grid[tuple(indices)] = 1
    return grid.flatten()

class TileCoder2:
    def __init__(self, state_space_ranges, num_tiles_per_dim, overlaps):
        self.state_space_ranges = state_space_ranges
        self.num_tiles_per_dim = num_tiles_per_dim
        self.overlaps = overlaps
        if isinstance(self.num_tiles_per_dim, int):
            self.num_tiles_per_dim = [self.num_tiles_per_dim] * len(self.state_space_ranges)
        if isinstance(self.overlaps, float):
            self.overlaps = [self.overlaps] * len(self.state_space_ranges)
        self.tiles = self.create_tiles()

    def create_tiles(self):
        tiles = []
        for i, (dim_range, num_tiles, overlap) in enumerate(
                zip(self.state_space_ranges, self.num_tiles_per_dim, self.overlaps)):
            curr_tiles = []
            low, high = dim_range
            r = (high - low)
            tile_width = r / (1 + overlap * (num_tiles - 1))
            stride = overlap * tile_width
            current = low
            while current + tile_width < high:
                curr_tiles.append((current, current + tile_width))
                current += stride
            curr_tiles.append((current, high))
            tiles.append(curr_tiles)
        return tiles

    def __call__(self, state):
        feature_vector = []
        for i, tile in enumerate(self.tiles):
            feature = [0] * len(tile)
            for j in range(len(tile)):
                if (tile[j][0] <= state[i] < tile[j][1]) or (j == len(tile) - 1 and state[i] == tile[j][1]):
                    feature[j] = 1
            feature_vector += feature
        return np.array(feature_vector, dtype=int)
tile_type = 2

num_tiles_per_dim = [60, 40]  # Example number of tiles per dimension
stride = 0.2  # Example stride between tiles
tile_coder = TileCoder2(lims, num_tiles_per_dim, stride)
state_size = len(tile_coder.tiles[1]) + len(tile_coder.tiles[0])

print(f"state size: {state_size}")


def evaluate_policy(policy):
    env = get_env()
    steps = 0
    episodes = 0

    while steps < 1000:
        state, _ = env.reset()
        state = tile_coder(state)
        state = state.reshape([1, state_size])
        done = False

        while not done and steps < 1000:
            # Policy network action selection
            actions_distribution = policy(state)
            action = np.random.choice(np.arange(len(actions_distribution)), p=actions_distribution)
            next_state, reward, terminated, truncated, _ = env.step(action)
            done = terminated or truncated
            next_state = tile_coder(next_state)
            next_state = next_state.reshape([1, state_size])
            state = next_state
            steps += 1

        episodes += 1 if done else 0

    return episodes

losses = []
rewards = []
evaluated_policy = []

def run():
    total_steps = 0
    action_size = env.action_space.n

    max_episodes = 3500
    max_steps = 201
    discount_factor = 0.95
    learning_rate_policy = 0.0005
    learning_rate_value = 0.005

    render = False

    # Instantiate policy and value networks
    tf.reset_default_graph()
    policy = PolicyNetwork(state_size, action_size, learning_rate_policy)
    value_net = ValueNetwork(state_size, learning_rate_value)

    with tf.Session() as sess:
        sess.run(tf.global_variables_initializer())
        solved = False
        episode_rewards = np.zeros(max_episodes)
        average_rewards = -float("inf")

        for episode in range(max_episodes):
            episode_transitions = []
            state, _ = env.reset()
            state = tile_coder(state)
            state = state.reshape([1, state_size])
            total_loss = 0.0

            for step in range(max_steps):
                # Policy network action selection
                actions_distribution = sess.run(policy.actions_distribution, {policy.state: state})
                action = np.random.choice(np.arange(len(actions_distribution)), p=actions_distribution)
                next_state, reward, terminated, truncated, _ = env.step(action)
                done = terminated or truncated
                next_state = tile_coder(next_state)
                next_state = next_state.reshape([1, state_size])

                if render:
                    env.render()

                action_one_hot = np.zeros(action_size)
                action_one_hot[action] = 1
                episode_rewards[episode] += reward

                value_estimate = sess.run(value_net.value_estimate, {value_net.state: state})[0, 0]
                next_value_estimate = sess.run(value_net.value_estimate, {value_net.state: next_state})[0, 0]
                next_value_estimate = reward + discount_factor * next_value_estimate * (1 - int(done))
                delta = next_value_estimate - value_estimate
                # Update value network
                feed_dict_value = {value_net.state: state, value_net.target: [[next_value_estimate]]}
                _, val_loss = sess.run([value_net.optimizer, value_net.loss], feed_dict_value)
                # Update policy network with advantage
                feed_dict_policy = {policy.state: state, policy.td_error: delta, policy.action: action_one_hot}
                _, policy_loss = sess.run([policy.optimizer, policy.loss], feed_dict_policy)
                total_loss += val_loss + policy_loss
                state = next_state

                if done:
                    if episode > 98:
                        # Calculate average rewards over the last 100 episodes
                        average_rewards = np.mean(episode_rewards[(episode - 99):episode + 1])

                    print(
                        "Episode {} Reward: {} Average over 100 episodes: {}".format(episode,
                                                                                    episode_rewards[episode],
                                                                                    round(average_rewards, 2)))

                    if average_rewards >= -110:
                        print(' Solved at episode: ' + str(episode))
                        solved = True
                    break

                total_steps += 1
                if total_steps % 25_000 == 0:
                    ep = evaluate_policy(lambda state: sess.run(policy.actions_distribution, {policy.state: state}))
                    print(f"evaluated_policy: {ep}")
                    evaluated_policy.append(ep)

            if solved:
                break

            losses.append(total_loss)
            rewards.append(episode_rewards[episode])

run()

plt.plot([loss / stepts for loss, stepts in zip(losses, rewards)])
plt.title('average losses over Episodes')
plt.xlabel('Episode')
plt.ylabel('avg_losses')
plt.show()

plt.plot(rewards)
plt.title('rewards over Episodes')
plt.xlabel('Episode')
plt.ylabel('reward')
plt.show()

plt.plot(evaluated_policy)
plt.title('evaluated_policy')
plt.xlabel('steps (in 25k)')
plt.ylabel('episodes per 1k steps')
plt.show()
