# Genetic-Algorithm
A Genetic Algorithm solution for The Dump Truck Problem created as a final project for the Engineering Optimization Course for the CEI track in the ITI.

The Genetic Algorithm used in this the Roulette Wheel selection via uniform crossover with mutation where the fitness is calculated by running a simulation that randomly selects loading time, weighing time and traveling time according to their probability, the fitness is calculated as 1/(totalcost+1) multiplied by a constant factor to to make the number look more natural.
The simulation and genetic algorithm were all coded from scratch without using an extenal library
features: It can use a dynamic distribution, calculates the cost of delay and picks the gene with the best overall cost, can run a single simulation and a genetic algorithm, displays the genetic algorithm results on a graph.


The default values are as follow:
Max number of trucks = 6
Max number of loaders = 2
Max Number of Scales = 2
Load = 10k m^3
Load per truck = 20 m^3

Cost of Truck = 1k/day
Cost of Loader = 2k/day
Cost of Scaler = 3k/day

![Loading Time Distribution](https://github.com/CSBebo/Genetic-Algorithm/blob/master/My%20Genetic%20Algorithm%20GUI/Loading%20Time.JPG)

![Traveling Time Distribution](https://github.com/CSBebo/Genetic-Algorithm/blob/master/My%20Genetic%20Algorithm%20GUI/Weighing%20Time.JPG)

![Traveling
Time Distribution](https://github.com/CSBebo/Genetic-Algorithm/blob/master/My%20Genetic%20Algorithm%20GUI/Travel%20Time.JPG)
