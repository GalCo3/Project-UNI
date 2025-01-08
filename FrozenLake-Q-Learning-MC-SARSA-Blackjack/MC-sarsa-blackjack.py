import numpy as np
import gymnasium as gym
import matplotlib.pyplot as plt
import json

def state_to_int(state):
    x, y, z = state
    
    num_x_values = 32
    num_y_values = 11
    num_z_values = 2

    # Ensure the values are within the specified range
    if not (0 <= x < num_x_values and 0 <= y < num_y_values and 0 <= z < num_z_values):
        raise ValueError("Invalid values for x, y, or z")
    
    hash_value = (x * num_y_values * num_z_values + y * num_z_values + z) % 704
    
    return hash_value

def int_to_state(state_int):
    
    num_x_values = 32
    num_y_values = 11
    num_z_values = 2
    
    # Ensure the value is within the specified range
    if not (0 <= state_int < 704):
        raise ValueError("Invalid value for state_int")
    
    # Perform operations to generate a state tuple
    x = state_int // (num_y_values * num_z_values)
    y = (state_int // num_z_values) % num_y_values
    z = state_int % num_z_values
    
    return (x, y, z)


def Transition_matrix(env: gym.Env, num_episodes: int):
    cells_withNoZero = set()
    transition_matrix1 = [[0 for _ in range(704)] for _ in range(704)]  # Transition matrix for action 0
    transition_matrix2 = [[0 for _ in range(704)] for _ in range(704)]  # Transition matrix for action 1
    rewards1 = [0 for _ in range(704)]  # Rewards for action 0
    rewards2 = [0 for _ in range(704)]  # Rewards for action 1


    for _ in range(num_episodes):
        state, _ = env.reset()
        done = False

        while not done:
            action = env.action_space.sample()
            next_state, reward, done, _, _ = env.step(action)

            if action == 0:
                transition_matrix1[state_to_int(state)][ state_to_int(next_state)] += 1
                cells_withNoZero.add((state_to_int(state), state_to_int(next_state)))
            else:
                transition_matrix2[state_to_int(state)][state_to_int(next_state)] += 1
                cells_withNoZero.add((state_to_int(state), state_to_int(next_state)))

            if done:
                if action == 0:
                    rewards1[state_to_int(next_state)] += reward
                else:
                    rewards2[state_to_int(next_state)] += reward
            state = next_state
        

    
    for row in transition_matrix1:
        row_sum = 0
        for i in range(len(row)):
            row_sum += row[i]
        for cell in row:
            if cell != 0:
                if row_sum!= cell:
                    print("success")
                cell = cell / row_sum
    for row in transition_matrix2:
        row_sum = 0
        for i in range(len(row)):
            row_sum += row[i]
        for cell in row:
            if cell != 0:
                cell = cell / row_sum
                
    return transition_matrix1, transition_matrix2, rewards1, rewards2







def greedy_policy_improvement_blackjack(state_action_values):
    policy = [0 for _ in range(704)]
    for state in range(704):
        policy[state] = np.argmax([state_action_values[(state, 0)], state_action_values[(state, 1)]])
    return policy

def greedy_policy_improvement_frozenlake(state_action_values):
    policy = [0 for _ in range(16)]
    for state in range(16):
        policy[state] = np.argmax([state_action_values[(state, 0)], state_action_values[(state, 1)], state_action_values[(state, 2)], state_action_values[(state, 3)]])
    return policy
    


def monte_carlo_policy_evaluation_blackjack(policy, env, num_episodes=1000,first_visit=True):
    
    state_action_returns = {}
    state_action_count = {}
    
    for _ in range(num_episodes):
        state, _ = env.reset()
        done = False
        episode = []
        
        while not done:
            state = state_to_int(state)
            action = policy[state]
            next_state, reward, done, _, _ = env.step(action)
            episode.append((state, action, reward))
            state = next_state
        
        G = 0
        for i in range(len(episode) - 1, -1, -1):
            state, action, reward = episode[i]
            G += reward
            
            if first_visit:
                if (state, action) not in [(s, a) for s, a, _ in episode[:i]]:
                    if (state, action) not in state_action_returns:
                        state_action_returns[(state, action)] = G
                        state_action_count[(state, action)] = 1
                    else:
                        state_action_returns[(state, action)] += G
                        state_action_count[(state, action)] += 1
            else:
                if (state, action) not in state_action_returns:
                    state_action_returns[(state, action)] = G
                    state_action_count[(state, action)] = 1
                else:
                    state_action_returns[(state, action)] += G
                    state_action_count[(state, action)] += 1
    
    
    state_action_values = {}
    for (state, action), returns_sum in state_action_returns.items():
        state_action_values[(state, action)] = returns_sum / state_action_count[(state, action)]

  
    for state in range(704):
        for action in range(2):
            if (state, action) not in state_action_values:
                state_action_values[(state, action)] = 0

    value_function = [0 for _ in range(704)]
    for state in range(704):
        value_function[state]  = state_action_values[(state,0)] + state_action_values[(state,1)]
    
    return value_function,state_action_values



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
            next_state, reward, done,terminate,_ = env.step(action)
            episode.append((state, reward))
            state = next_state
            if done or terminate:
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
    
            



def epsilon_greedy_action_choice_blackjack(state, policy, epsilon=0.3):
    if np.random.random() < epsilon:
        return np.random.choice([0, 1])
    else:
        return policy[state]
    
def epsilon_greedy_action_choice_frozenLake(state, Q, epsilon=0.3):
    if np.random.random() < epsilon:
        return np.random.choice([0, 1, 2, 3])
    else:
        return np.argmax([Q[(state, 0)], Q[(state, 1)], Q[(state, 2)], Q[(state, 3)]])
    

def sarasa_policy_evaluation(state_action_values,policy, env, num_episodes=1000):
    
    
    for _ in range(num_episodes):
        #choose random state
        state, _ = env.reset()
        state = state_to_int(state)
        action = epsilon_greedy_action_choice_blackjack(state, policy)
        done = False
       
        while not done:
            next_state, reward, done, _, _ = env.step(action)
            
            next_state = state_to_int(next_state)
            next_action = epsilon_greedy_action_choice_blackjack(next_state, policy)
            state_action_values[(state, action)] = state_action_values[(state, action)] + 0.1 * (reward + state_action_values[(next_state, next_action)] - state_action_values[(state, action)]) 
            state = next_state
            action = next_action
            

    return state_action_values

def monte_carlo_policy_iteration(env, num_episodes=1000, num_iterations=100, first_visit=True):

    policy = []
    avg_value = []

    for state in range(704):
        if int_to_state(state)[0] < 21:
            policy.append(1)
        else:
            policy.append(0)

    for i in range(num_iterations):
        
        value_function,state_action_value = monte_carlo_policy_evaluation_blackjack(policy, env, num_episodes, first_visit)
        policy = greedy_policy_improvement_blackjack(state_action_value)
        if i<20:
            avg_value.append(0.0)
            for player_sum in range(13,17):
                for dealer_card in range(7,9):
            
                    state  = state_to_int((player_sum, dealer_card, 0))
                    avg_value[i] += value_function[state]
            avg_value[i] = avg_value[i] / 8.0

        
    return policy, avg_value

def epsilon_greedy_policy_improvement(state_action_values, epsilon=0.3):
    policy = [0 for _ in range(704)]
    for state in range(704):
        if np.random.random() < epsilon:
            policy[state] = np.random.choice([0, 1])
        else:
            policy[state] = np.argmax([state_action_values[(state, 0)], state_action_values[(state, 1)]])
    return policy

def sarsa_policy_iteration(env, num_iterations=100):
 
    policy = []
    avg_value = []

    for state in range(704):
        if int_to_state(state)[0] < 21:
            policy.append(1)
        else:
            policy.append(0)
    
    state_action_values = {}

    for state in range(704):
        for action in range(2):
            state_action_values[(state, action)] = 0

    for i in range(num_iterations):
                
        state_action_values = sarasa_policy_evaluation(state_action_values,policy, env, 1000)
        policy = epsilon_greedy_policy_improvement(state_action_values)
        
        if i<20:
            avg_value.append(0.0)
            for player_sum in range(13,17):
                for dealer_card in range(7,9):
                    state  = state_to_int((player_sum, dealer_card, 0))
                    avg_value[i] += state_action_values[(state, policy[state])]
            avg_value[i] = avg_value[i] / 8.0
        
    return policy, avg_value










    

if __name__ == "__main__":
    # monte carlo policy iteration
    env = gym.make('Blackjack-v1', natural=False, sab=False)
    policy1,avg_value1 = monte_carlo_policy_iteration(env, num_episodes=1000, num_iterations=20, first_visit=True)
    policy2,avg_value2 = monte_carlo_policy_iteration(env, num_episodes=1000, num_iterations=20, first_visit=False)
    policy3,avg_value3 = sarsa_policy_iteration(env, num_iterations=20)

    #plot the avaerage values (1,2,3)
    plt.plot([x for x in avg_value1], label="MC First Visit")
    plt.plot([x for x in avg_value2], label="MC Every Visit")
    plt.plot([x for x in avg_value3], label="SARSA")
    plt.legend()
    plt.show()


    


    
    
    





    
 