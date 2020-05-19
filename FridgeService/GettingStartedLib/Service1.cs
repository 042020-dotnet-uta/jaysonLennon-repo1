using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace GettingStartedLib
{
    [ServiceContract]
    public class FridgeService : IFridge
    {
        private Dictionary<string, int> Fruits = new Dictionary<string, int>();
        public int AddFruit(string fruitType, int count)
        {
            if (!this.Fruits.ContainsKey(fruitType)) this.Fruits.Add(fruitType, 0);
            this.Fruits[fruitType] += count;
            return this.Fruits[fruitType];
        }

        public int RemoveFruit(string fruitType, int count)
        {
            if (this.Fruits.ContainsKey(fruitType))
            {
                var total = this.Fruits[fruitType];
                this.Fruits[fruitType] = (total - count) >= 0 ? (total - count) : 0;
                return this.Fruits[fruitType];
            } else
            {
                return 0;
            }
        }

        public int TotalFruit()
        {
            return this.Fruits.Values.Sum();
        }
    }
}