using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advanced_Combat_Tracker;

namespace BnS_ACT_Plugin.Test
{
    public class TestACTWrapper : BNS_ACT_Plugin.IACTWrapper
    {
        public class CombatAction
        {
            public int SwingType;
            public bool Critical;
            public string Special;
            public string Attacker;
            public string theAttackType;
            public Dnum Damage;
            public DateTime Time;
            public int TimeSorter;
            public string Victim;
            public string theDamageType;

            public CombatAction(int SwingType, bool Critical, string Special, string Attacker, string theAttackType, Dnum Damage, DateTime Time, int TimeSorter, string Victim, string theDamageType)
            {
                this.SwingType = SwingType;
                this.Critical = Critical;
                this.Special = Special;
                this.Attacker = Attacker;
                this.theAttackType = theAttackType;
                this.Damage = Damage;
                this.Time = Time;
                this.TimeSorter = TimeSorter;
                this.Victim = Victim;
                this.theDamageType = theDamageType;
            }
        }

        public class Encounter
        {
            public DateTime Time;
            public string Attacker;
            public string Victim;

            public Encounter(DateTime Time, string Attacker, string Victim)
            {
                this.Time = Time;
                this.Attacker = Attacker;
                this.Victim = Victim;
            }
        }

        public int GlobalTimeSorter
        {
            get; set;
        }

        public List<CombatAction> CombatActions
        {
            get; protected set;
        }

        public Encounter CurrentEncounter
        {
            get; protected set;
        }

        public TestACTWrapper()
        {
            CombatActions = new List<CombatAction>();
            CurrentEncounter = null;
        }

        public void AddCombatAction(int SwingType, bool Critical, string Special, string Attacker, string theAttackType, Dnum Damage, DateTime Time, int TimeSorter, string Victim, string theDamageType)
        {
            CombatActions.Add(new CombatAction(SwingType, Critical, Special, Attacker, theAttackType, Damage, Time, TimeSorter, Victim, theDamageType));
        }

        public bool SetEncounter(DateTime Time, string Attacker, string Victim)
        {
            // for now, just return true.
            return true;
        }
    }
}
