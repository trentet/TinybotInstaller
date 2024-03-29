﻿using System.Collections.Generic;
using TinybotInstaller.Tasks;

namespace TinybotInstaller
{
    class Component
    {
        public string Name { get; set; }
        public Dictionary<int, ComponentTask> ComponentTasks { get; set; } = new Dictionary<int, ComponentTask>();

        public Component(string name)
        {
            this.Name = name;
        }

        public Component(string name, Dictionary<int, ComponentTask> componentTasks)
        {
            this.Name = name;
            this.ComponentTasks = componentTasks;
        }
    }
}
