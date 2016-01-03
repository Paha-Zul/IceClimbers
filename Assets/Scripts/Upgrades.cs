using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

class Upgrades {
    private static Dictionary<string, Upgrade> upgradeMap = new Dictionary<string, Upgrade>();
    public static bool populated { get; set; }

    public static Upgrade addUpgrade(string name, string prefName, int cost, int costIncr, int max, int curr, Text costText, Text currText) {
        Upgrade upgrade = new Upgrade(prefName, cost, costIncr, max, curr, costText, currText);
        upgradeMap.Add(name, upgrade);
        return upgrade;
    }

    public static Upgrade refreshOrAdd(string name, string prefName, int cost, int costIncr, int max, int curr, Text costText, Text currText) {
        Upgrade upgrade;
        upgradeMap.TryGetValue(name, out upgrade);
        if(upgrade == null) {
            //If we didn't get it out of the dictionary, add a new one.
            upgrade = new Upgrade(prefName, cost, costIncr, max, curr, costText, currText);
            upgradeMap.Add(name, upgrade);
        } else {
            //Gotta update these because they are new objects when the MainMenu is reloaded.
            upgrade.costText = costText;
            upgrade.currText = currText;
        }

        return upgrade;
    }

    public static Upgrade getUpgrade(string name) {
        Upgrade upgrade;
        upgradeMap.TryGetValue(name, out upgrade);
        return upgrade;
    }

    public static Dictionary<string, Upgrade> getDictionary() {
        return upgradeMap;
    }

    public class Upgrade {
        public string prefName;
        public int cost, costIncr, max, curr;
        public Text costText, currText;

        public Upgrade(string prefName, int cost, int costIncr, int max, int curr, Text costText, Text currText) {
            this.prefName = prefName;
            this.cost = cost;
            this.costIncr = costIncr;
            this.max = max;
            this.curr = curr;
            this.costText = costText;
            this.currText = currText;
        }
    }
}
