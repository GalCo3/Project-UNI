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



def approximate_policy_evaluation(policy, transition_matrix1, transition_matrix2,rewards1,rewards2, value_function, k=5):
    
        
    val_func_for_stick = [0 for _ in range(704)]
    val_func_for_hit = [0 for _ in range(704)]
    for _ in range(k):
        new_value_function = [0 for _ in range(704)]
        new_val_func_for_stick = [0 for _ in range(704)]
        new_val_func_for_hit = [0 for _ in range(704)]

        for state in range(704):
            action = policy[state]
            if action == 0:
                new_value_function[state] = rewards1[state]
                new_val_func_for_stick[state] = rewards1[state]
            else:
                new_value_function[state] = rewards2[state]
                new_val_func_for_hit[state] = rewards2[state]

            for next_state in range(704):
                new_value_function[state] += transition_matrix1[state][next_state] * value_function[next_state]
                new_val_func_for_stick[state] += transition_matrix1[state][next_state] * value_function[next_state]
                new_value_function[state] += transition_matrix2[state][next_state] * value_function[next_state]
                new_val_func_for_hit[state] += transition_matrix2[state][next_state] * value_function[next_state]


        value_function = new_value_function
        val_func_for_stick = new_val_func_for_stick
        val_func_for_hit = new_val_func_for_hit

    
    return value_function, val_func_for_stick, val_func_for_hit



def greedy_policy_improvement(val_func_for_stick,val_func_for_hit):

    new_policy = []
    for state in range(704):
        if val_func_for_stick[state] >= val_func_for_hit[state]:
            new_policy.append(0)
        else:
            new_policy.append(1)
    return new_policy
    

    
def modified_policy_iteration(env, transition_matrix1, transition_matrix2,rewards1,rewards2, k=5, num_iterations=100):
 
    policy = []
    policy_history = []
    value_function_history = []

    #first poilicy will be , if i have less than 21 i will hit
    for state in range(704):
        if int_to_state(state)[0] < 21:
            policy.append(1)
        else:
            policy.append(0)
            
    value_function = [0 for _ in range(704)]

    for _ in range(num_iterations):
        old_policy = policy
        value_function,val_func_for_stick,val_func_for_hit = approximate_policy_evaluation(policy, transition_matrix1, transition_matrix2,rewards1,rewards2, value_function, k)
        
        policy = greedy_policy_improvement(val_func_for_stick,val_func_for_hit)
        
        allSame = True
        for i in range(len(old_policy)):
            if old_policy[i] != policy[i]:
                allSame = False
                break

        if allSame:
            break

        policy_history.append(policy)
        value_function_history.append(value_function)
    return policy, policy_history, value_function_history


def execute_policy(env:gym.Env, policy):
    state = env.reset()
    done = False
    while not done:
        action = policy[state]
        state, _, done, _, _ = env.step(action)
    

def value_function_q3(value_function, state):
    if state is tuple:
        state = state_to_int(state)
    print("Value function for state", state, "is", value_function[state])

def save_load_json():
    try:
        #try to load from json file
        with open('transition_matrix1.json', 'r') as f:
            transition_matrix1 = json.load(f)
        with open('transition_matrix2.json', 'r') as f:
            transition_matrix2 = json.load(f)
        with open('rewards1.json', 'r') as f:
            rewards1 = json.load(f)
        with open('rewards2.json', 'r') as f:
            rewards2 = json.load(f)
        with open('policy_history.json', 'r') as f:
            policy_history = json.load(f)
        with open('optimal_policy.json', 'r') as f:
            optimal_policy = json.load(f)
        with open('value_function_history.json', 'r') as f:
            value_function_history = json.load(f)
    except:
        num_episodes = 30000
        num_iterations = 10
        k = 5
        transition_matrix1, transition_matrix2, rewards1,rewards2 = Transition_matrix(env, num_episodes)
        optimal_policy, policy_history,value_function_history= modified_policy_iteration(env, transition_matrix1, transition_matrix2,rewards1,rewards2, k,  num_iterations)
        with open('transition_matrix1.json', 'w') as f:
            json.dump(transition_matrix1, f)

        with open('transition_matrix2.json', 'w') as f:
            json.dump(transition_matrix2, f)

        with open('rewards1.json', 'w') as f:
            json.dump(rewards1, f)

        with open('rewards2.json', 'w') as f:
            json.dump(rewards2, f)

        with open('policy_history.json', 'w') as f:
            json.dump(policy_history, f)

        with open('optimal_policy.json', 'w') as f:
            json.dump(optimal_policy, f)
        
        with open('value_function_history.json', 'w') as f:
            json.dump(value_function_history, f)
    return transition_matrix1, transition_matrix2, rewards1,rewards2, optimal_policy, policy_history, value_function_history

def plot_value_function_avg(value_function_history):
    avg_list = []
    for i, value_function in enumerate(value_function_history, start=1):
        avg = sum(value_function) / len(value_function)
        avg_list.append(avg)
    
    plt.plot(range(1, len(avg_list) + 1), avg_list)
    plt.xticks(range(1, len(avg_list) + 1))
    
    plt.ylabel('Average Value Function')
    plt.xlabel('Iteration')
    plt.show()





def plot_policy_table1(optimal_policy_states):
    # Filter out the states where state[2] == 0
    filtered_states = [state for state in optimal_policy_states if state[2] == 0]

    # Create a table of size 17 * 9
    table_data = np.zeros((17, 9), dtype=int)

    # Update the table based on optimal policy states
    for state in filtered_states:
        table_data[state[1] - 4][state[0] - 2] = 1

    # Plotting the policy table as a regular table
    fig, ax = plt.subplots()
    ax.axis('off')  # Turn off axis labels and ticks

    table = ax.table(cellText=table_data, loc='center', cellLoc='center', colWidths=[0.08]*9)

    # Adjust font size if needed
    table.auto_set_font_size(False)
    table.set_fontsize(10)

    # Adding axis names and indexes
    ax.annotate('Player Sum', xy=(0.5, -0.1), ha='center', va='center', xycoords='axes fraction', fontsize=12)
    ax.annotate('Dealer Showing', xy=(-0.1, 0.5), ha='center', va='center', xycoords='axes fraction', rotation='vertical', fontsize=12)

    for i in range(4, 21):
        ax.annotate(str(i), xy=(i - 2 + 0.5, -0.15), ha='center', va='center', fontsize=10)

    for j in range(2, 11):
        ax.annotate(str(j), xy=(-0.15, j - 4 + 0.5), ha='center', va='center', fontsize=10)

    plt.show()


    

if __name__ == "__main__":
    env = gym.make('Blackjack-v1', natural=False, sab=False)
    transition_matrix1, transition_matrix2, rewards1,rewards2, optimal_policy, policy_history, value_function_history = save_load_json()

    #plotting the value function
    plot_value_function_avg(value_function_history)
    # matrix 17* 9
    matrix = np.zeros((17, 9), dtype=float)
    for state in range(4,20):
        for dealer in range(2,11):
            matrix[state-4][dealer-2] = optimal_policy[state_to_int((state,dealer,0))]
    # plot the matrix as ta
    fig, ax = plt.subplots()
    ax.axis('off')  # Turn off axis labels and ticks
    table = ax.table(cellText=matrix, loc='center', cellLoc='center', colWidths=[0.08]*9)
    # Adjust font size if needed
    table.auto_set_font_size(False)
    table.set_fontsize(10)
    # Adding axis names and indexes
    
    ax.annotate('Dealer Showing', xy=(0.5, -0.1), ha='center', va='center', xycoords='axes fraction', fontsize=12)
    ax.annotate('Player Sum', xy=(-0.1, 0.5), ha='center', va='center', xycoords='axes fraction', rotation='vertical', fontsize=12)
    for i in range(4, 21):
        ax.annotate(str(i), xy=(i - 2 + 0.5, -0.15), ha='center', va='center', fontsize=10)
    for j in range(2, 11):
        ax.annotate(str(j), xy=(-0.15, j - 4 + 0.5), ha='center', va='center', fontsize=10)
    plt.show()
    


    
    
    





    
 