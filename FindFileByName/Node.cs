﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace FindFileByName
{
    public class Node
    {
        public string Name { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }

        private static char[] split = new char[] { '\\' };

        public void Add(string fileName, Dispatcher dispatcher)
        {
            if (Nodes == null)
                Nodes = new ObservableCollection<Node>();

            string[] dir = fileName.Split(split, 2);

            if (dir.Length > 1)
            {
                foreach (Node node in Nodes)
                {
                    if (node.Name.Equals(dir[0]))
                    {
                        node.Add(dir[1], dispatcher);
                        return;
                    }
                }

                Node newNode = new Node { Name = dir[0] };
                dispatcher.Invoke(() => Nodes.Add(newNode));
                newNode.Add(dir[1], dispatcher);
                return;
            }

            dispatcher.Invoke(()=> Nodes.Add(new Node() { Name = dir[0] }));
        }
    }
}
