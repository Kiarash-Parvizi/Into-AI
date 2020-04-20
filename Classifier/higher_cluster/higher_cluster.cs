using System;
using System.Collections.Generic;

namespace classifier {

    //--------------------- Dep
    public class node {
        public char type = '-';
        public int weight = 1;
        public double[] vec;
        // Constructor
        public node(double[] vec, char type) {
            this.vec = new double[vec.Length];
            vec.CopyTo(this.vec, 0);
            this.type = type;
        }
        public node(node o) {
            this.vec = new double[o.vec.Length];
            o.vec.CopyTo(this.vec, 0);
            this.type = o.type;
        }
        public double dist(node o) {
            if (vec.Length != o.vec.Length) {
                throw new Exception();
            }
            double dist = 0, d;
            for (int i = 0; i < vec.Length; i++) {
                d = vec[i]-o.vec[i];
                dist += d*d;
            }
            return Math.Sqrt(dist);
        }

        // Merge
        public void merge(node o) {
            if (o.vec.Length != vec.Length) {
                throw new Exception();
            }
            for (int i = 0; i < vec.Length; i++) {
                vec[i] = (vec[i]*weight + o.vec[i]*o.weight)/(weight+o.weight);
            }
            weight += o.weight;
        }
    }
    //---------------------
    //---------------------

    public class higher_cluster {
        int vec_space = 0;
        List<node> nodes = new List<node>();
        List<node> opt_nodes = new List<node>();
        public higher_cluster(int vec_place) {
            this.vec_space = vec_place;
        }
        // Funcs
        // adds a new node
        public void add(node n) {
            if (n.vec.Length != vec_space) {
                throw new Exception();
            }
            nodes.Add(n);
        }
        // prints all the nodes
        public void print(bool only_opt_nodes = false) {
            Console.WriteLine("{\nsize = " + (only_opt_nodes ? opt_nodes : nodes).Count);
            foreach (var n in (only_opt_nodes ? opt_nodes : nodes)) {
                Console.Write("\tNode = { " + n.type + " -> ");
                foreach (var v in n.vec) {
                    Console.Write(v+" ");
                }
                Console.WriteLine("}");
            }
            Console.WriteLine("}");
        }
        // classify
        public char classify(node NODE) {
            // Preprocessing
            optimize();
            // Search
            if (opt_nodes.Count == 0) {
                throw new Exception();
            }
            char min_dist_cat='-'; double min_dist = Double.MaxValue;
            foreach (var n in opt_nodes) {
                var dist = NODE.dist(n);
                if (dist < min_dist) {
                    min_dist_cat = n.type;
                    min_dist = dist;
                }
            }
            return min_dist_cat;
        }
        // BackUp
        public List<node> BackUp() {
            return new List<node>(nodes);
        }
        // reset_weights
        void reset_weights() {
            foreach (var v in nodes) {
                v.weight = 1;
            }
        }
        // Optimize
        public void optimize() {
            bool[] removed = new bool[nodes.Count];
            opt_nodes.Clear();
            // Iteration
            for (int i = 0; i < nodes.Count; i++) {
                if (!removed[i]) {
                    var NODE = new node(nodes[i]);
                    var min_dist = Double.MaxValue;
                    // Finding min_dist between NODE and nodes of other types
                    for (int j = 0; j < nodes.Count; j++) {
                        if (i == j) { continue; }
                        if (nodes[j].type != NODE.type) {
                            var dist = NODE.dist(nodes[j]);
                            if (dist < min_dist) {
                                min_dist = dist;
                            }
                        }
                    }
                    var cluster_range = min_dist;
					removed[i] = true;
                    // ---------------
                    //Console.WriteLine("> cluster_range: " + cluster_range);
                    // merge with nodes in cluster_range
                    for (int j = 0; j < nodes.Count; j++) {
                        if (!removed[j] && nodes[j].type == NODE.type) {
                            var dist = NODE.dist(nodes[j]);
                            if (dist < cluster_range) {
                                removed[j] = true;
                                NODE.merge(nodes[j]);
                            }
                        }
                    }
                    // ---------------

                    // Assign
                    opt_nodes.Add(NODE);
                    // ---------------
                }
            }
        }
    }

}


// /*
namespace test {
    using classifier;
    public static class Program {
        public static void Main() {
            var h = new higher_cluster(3);
            h.add(new node(new double[3]{1, 2.6, 3}, '1'));
            h.add(new node(new double[3]{3, 5, 5}, '1'));

            h.add(new node(new double[3]{1, -5, -3}, '2'));
            h.add(new node(new double[3]{2, -2, -2}, '2'));
            h.add(new node(new double[3]{15, -5, -3}, '2'));
            h.add(new node(new double[3]{20, -2, -2}, '2'));

            h.add(new node(new double[3]{-2, -2, 1}, '3'));
            h.add(new node(new double[3]{-3, -4, -2}, '3'));
            h.add(new node(new double[3]{0, -2, -1}, '3'));
            //
            //
            var res = h.classify(new node(new double[3]{-1, -2, 0}, '-'));
            h.print();
            h.print(true);
            Console.WriteLine("\nClassified as -> " + res);
        }
    }
}
// */