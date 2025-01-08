import numpy as np
import gymnasium as gym
import matplotlib.pyplot as plt
import json

def epsilon_greedy_action_choice_frozenLake(state, Q, epsilon):
    if np.random.rand() < epsilon:
        return np.random.choice(4)  # 4 actions in FrozenLake
    else:
        return np.argmax([Q[(state, a)] for a in range(4)])

def greedy_policy_improvement_frozenlake(Q):
    policy = {}
    for state in range(16):
        policy[state] = np.argmax([Q[(state, a)] for a in range(4)])
    return policy

def monte_carlo_policy_evaluation_frozenLake(env, policy, num_episodes=1000):
    returns_sum = {}
    returns_count = {}
    V = {}

    for _ in range(num_episodes):
        state,_ = env.reset()
        episode = []

        # Generate an episode
        while True:
            action = policy[state]
            next_state, reward, done,_ ,_ = env.step(action)
            episode.append((state, reward))
            state = next_state
            if done:
                break

        # Update state values using first-visit Monte Carlo
        visited_states = set()
        for t, (state, reward) in enumerate(episode):
            if state not in visited_states:
                visited_states.add(state)
                G = sum([step[1] for step in episode[t:]])
                if state not in returns_sum:
                    returns_sum[state] = 0
                    returns_count[state] = 0
                returns_sum[state] += G
                returns_count[state] += 1
                V[state] = returns_sum[state] / returns_count[state]

    return V

def Q_Learning(env: gym.Env, num_episodes=100000, gamma=0.95, epsilon=0.3, alpha=0.1):
    Q = {}
    returns = []

    for state in range(16):
        for action in range(4):
            Q[(state, action)] = 0

    for i in range(num_episodes):
        if (i + 1) % 1000 == 0:
            greedy_policy = greedy_policy_improvement_frozenlake(Q)
            V_monte = monte_carlo_policy_evaluation_frozenLake(env, greedy_policy, 1000)
            returns.append(V_monte[0])

        state,_ = env.reset()
        done = False
        terminate = False
        while not done and not terminate:
            action = epsilon_greedy_action_choice_frozenLake(state, Q, epsilon)
            next_state, reward, done, terminate, _ = env.step(action)
            Q[(state, action)] = Q[(state, action)] + alpha * (
                    reward + gamma * np.max([Q[(next_state, a)] for a in range(4)]) - Q[(state, action)])
            state = next_state

    #plot the returns list
    # Plot the returns list
    return Q, returns

# Example usage
try:
    #open from file
    with open('returns.json', 'r') as f:
        returns = json.load(f)
    
except:
    env = gym.make('FrozenLake-v1')
    Q,returns = Q_Learning(env)


    with open('returns.json', 'w') as f:
        json.dump(returns, f)
    
num_points = min(len(returns), 100)
x_values = range(10000, 10000 + num_points * 10000, 10000)

# Plot the returns list
# make x that will be numbers of iterations
x_values = list(x_values)
plt.plot(x_values, returns[:num_points])
plt.xlabel('Iterations (in thousands)')
plt.ylabel('Value of Greedy Policy')
plt.xticks(x_values, labels=x_values, rotation='vertical')
plt.title('Q-Learning Performance')
plt.show()