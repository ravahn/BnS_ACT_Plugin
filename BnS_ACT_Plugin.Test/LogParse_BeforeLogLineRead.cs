using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

using Advanced_Combat_Tracker;

namespace BnS_ACT_Plugin.Test
{
    [TestClass]
    public class LogParse_BeforeLogLineRead
    {
        TestACTWrapper dependency = null;

        [TestInitialize]
        public void LogParse_LogRead_Initialize()
        {
            // set up ACT proxy class
            dependency = new TestACTWrapper();
            dependency.GlobalTimeSorter = 100000;
            BNS_ACT_Plugin.LogParse.Initialize(dependency);
        }


        [TestMethod]
        public void LogParse_LogRead_yourdamage_large()
        {
            // Input
            string testLine = "15:11:33.635|2B|Blazing Palm hit Blackram Landing Soldier for 41,224 damage.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "You");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 41224);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Blazing Palm");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Blackram Landing Soldier");
        }

        [TestMethod]
        public void LogParse_LogRead_yourdamage_simple()
        {
            // Input
            string testLine = "15:11:33.635|2B|Blazing Palm hit Blackram Landing Soldier for 413 damage.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "You");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 413);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Blazing Palm");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Blackram Landing Soldier");
        }
        
        [TestMethod]
        public void LogParse_LogRead_yourdamage_critical()
        {
            string testLine = "15:11:33.635|2C|Blazing Palm critically hit Blackram Landing Soldier for 736 damage.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "You");
            Assert.AreEqual(dependency.CombatActions[0].Critical, true);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 736);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Blazing Palm");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Blackram Landing Soldier");
        }
        
        [TestMethod]
        public void LogParse_LogRead_yourdamage_drain()
        {
            // todo: correct log code
            string testLine = "15:11:33.635|FF|Dragonwhorl hit Sajifi for 283 damage, draining 283 HP and 1 Focus.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 2);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "You");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 283);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Dragonwhorl");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Sajifi");

            Assert.AreEqual(dependency.CombatActions[1].Attacker, "You");
            Assert.AreEqual(dependency.CombatActions[1].Critical, false);
            Assert.AreEqual(dependency.CombatActions[1].Damage.Number, 283);
            Assert.AreEqual(dependency.CombatActions[1].Special, "Drain");
            Assert.AreEqual(dependency.CombatActions[1].SwingType, (int)SwingTypeEnum.Healing);
            Assert.AreEqual(dependency.CombatActions[1].theAttackType, "Dragonwhorl");
            Assert.AreEqual(dependency.CombatActions[1].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[1].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[1].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[1].Victim, "You");         
        }
        /*
        // Ember debuffs not implemented
        [TestMethod]
        public void LogParse_LogRead_debuff2_simple()
        {
            string testLine = "15:11:33.635|FF|Blazing Palm hit Blackram Landing Soldier and inflicted Ember.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 2);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "You");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 283);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Dragonwhorl");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Sajifi");

            Assert.IsFalse(m.Groups["actor"].Success);
            Assert.IsTrue(m.Groups["skill"].Success && m.Groups["skill"].Value == "Blazing Palm");
            Assert.IsTrue(m.Groups["critical"].Success && m.Groups["critical"].Value == "hit");
            Assert.IsTrue(m.Groups["target"].Success && m.Groups["target"].Value == "Blackram Landing Soldier");
            Assert.IsTrue(m.Groups["debuff"].Success && m.Groups["debuff"].Value == "Ember");
            Assert.IsFalse(m.Groups["debuff2"].Success);
        }*/
        /*
        [TestMethod]
        public void LogParse_LogRead_debuff2_resisted()
        {
            string testLine = "15:11:33.635|FF|PLAYERNAME&apos;s Dragonfrost hit Mercenary Enforcer but Chill was resisted.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.IsTrue(m.Groups["actor"].Success && m.Groups["actor"].Value == "PLAYERNAME");
            Assert.IsTrue(m.Groups["skill"].Success && m.Groups["skill"].Value == "Dragonfrost");
            Assert.IsTrue(m.Groups["critical"].Success && m.Groups["critical"].Value == "hit");
            Assert.IsTrue(m.Groups["target"].Success && m.Groups["target"].Value == "Mercenary Enforcer");
            Assert.IsFalse(m.Groups["debuff"].Success);
            Assert.IsTrue(m.Groups["debuff2"].Success && m.Groups["debuff2"].Value == "Chill");
        }*/
        /*
        [TestMethod]
        public void LogParse_LogRead_debuff2_nodamage_inflicted()
        {
            string testLine = "15:11:33.635|FF|PLAYER NAME&apos;s Leading Palm inflicted Bleed.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.IsTrue(m.Groups["actor"].Success && m.Groups["actor"].Value == "PLAYER NAME");
            Assert.IsTrue(m.Groups["skill"].Success && m.Groups["skill"].Value == "Leading Palm");
            Assert.IsFalse(m.Groups["critical"].Success);
            Assert.IsFalse(m.Groups["target"].Success);
            Assert.IsTrue(m.Groups["debuff"].Success && m.Groups["debuff"].Value == "Bleed");
            Assert.IsFalse(m.Groups["debuff2"].Success);
        }*/

        /*
        [TestMethod]
        public void LogParse_LogRead_debuff_simple()
        {
            string testLine = "15:11:33.635|FF|Assault Unit Mercenary receives Chill.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.IsTrue(m.Groups["target"].Success && m.Groups["target"].Value == "Assault Unit Mercenary");
            Assert.IsTrue(m.Groups["skill"].Success && m.Groups["skill"].Value == "Chill");
        }
        */

        [TestMethod]
        public void LogParse_LogRead_incomingdamage1_simple()
        {
            string testLine = "15:11:33.635|FF|Assault Unit Mercenary received 1384 damage from PLAYERNAME&apos;s Sundering Sword.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "PLAYERNAME");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 1384);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Sundering Sword");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Assault Unit Mercenary");
        }

        [TestMethod]
        public void LogParse_LogRead_incomingdamage1_HPDrain_Crit()
        {
            string testLine = "15:11:33.635|FF|Deputy Mercenary received 1889 Critical damage and 963 HP drain from PLAYERNAME&apos;s Poison Breath.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 2);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "PLAYERNAME");
            Assert.AreEqual(dependency.CombatActions[0].Critical, true);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 1889);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Poison Breath");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Deputy Mercenary");

            Assert.AreEqual(dependency.CombatActions[1].Attacker, "PLAYERNAME");
            Assert.AreEqual(dependency.CombatActions[1].Critical, false);
            Assert.AreEqual(dependency.CombatActions[1].Damage.Number, 963);
            Assert.AreEqual(dependency.CombatActions[1].Special, "Drain");
            Assert.AreEqual(dependency.CombatActions[1].SwingType, (int)SwingTypeEnum.Healing);
            Assert.AreEqual(dependency.CombatActions[1].theAttackType, "Poison Breath");
            Assert.AreEqual(dependency.CombatActions[1].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[1].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[1].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[1].Victim, "PLAYERNAME");
        }

        [TestMethod]
        public void LogParse_LogRead_incomingdamage1_FocusDrain()
        {
            string testLine = "15:11:33.635|FF|Ferocious Attack Bear received 1130 damage and 1 Focus drain from PLAYERNAME&apos;s Blazing Palm.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "PLAYERNAME");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 1130);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Blazing Palm");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Ferocious Attack Bear");
        }

        [TestMethod]
        public void LogParse_LogRead_incomingdamage1_damage_knockdown()
        {
            string testLine = "15:11:33.635|FF|Assassin Guard received 287 damage and Knockdown from PLAYERNAME&apos;s Leg Sweep.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "PLAYERNAME");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 287);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Leg Sweep");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Assassin Guard");            
        }
        
        [TestMethod]
        public void LogParse_LogRead_incomingdamage1_damage_complex()
        {
            string testLine = "15:11:33.635|FF|Sac Spider received 346 damage, 36 HP drain, and 1 Focus drain from PLAYERNAME&apos;s Blazing Palm. ";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));


            Assert.AreEqual(dependency.CombatActions.Count, 2);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "PLAYERNAME");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 346);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Blazing Palm");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Sac Spider");

            Assert.AreEqual(dependency.CombatActions[1].Attacker, "PLAYERNAME");
            Assert.AreEqual(dependency.CombatActions[1].Critical, false);
            Assert.AreEqual(dependency.CombatActions[1].Damage.Number, 36);
            Assert.AreEqual(dependency.CombatActions[1].Special, "Drain");
            Assert.AreEqual(dependency.CombatActions[1].SwingType, (int)SwingTypeEnum.Healing);
            Assert.AreEqual(dependency.CombatActions[1].theAttackType, "Blazing Palm");
            Assert.AreEqual(dependency.CombatActions[1].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[1].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[1].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[1].Victim, "PLAYERNAME");            
        }

        [TestMethod]
        public void LogParse_LogRead_incomingdamage1_resist()
        {
            string testLine = "15:11:33.635|FF|Cobalt Widow received 1543 damage from PLAYERNAME&apos;s Strike but resisted Daze.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "PLAYERNAME");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 1543);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Strike");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Cobalt Widow");
        }

        
        [TestMethod]
        public void LogParse_LogRead_incomingdamage1_resist2()
        {
            string testLine = "15:11:33.635|FF|Scarlet Widow received 1943 Critical damage from PLAYERNAME&apos;s Strike, but resisted Daze effect.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "PLAYERNAME");
            Assert.AreEqual(dependency.CombatActions[0].Critical, true);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 1943);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Strike");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Scarlet Widow");
        }
        [TestMethod]
        public void LogParse_LogRead_incomingdamage2_Blocked()
        {
            string testLine = "15:11:33.635|FF|Blocked Mercenary Enforcer&apos;s Shuriken but received 247 damage.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "Mercenary Enforcer");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 247);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Shuriken");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "You");            
        }
        /*
        [TestMethod]
        public void LogParse_LogRead_incomingdamage2_targetdebuff()
        {
            string testLine = "15:11:33.635|FF|PLAYERNAME blocked Mercenary Enforcer&apos;s Shadow Slash but received Shadow.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "Mercenary Enforcer");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 247);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Shadow Slash");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "You");

            Assert.IsTrue(m.Groups["target"].Success && m.Groups["target"].Value == "PLAYERNAME");
            Assert.IsTrue(m.Groups["actor"].Success && m.Groups["actor"].Value == "Mercenary Enforcer");
            Assert.IsTrue(m.Groups["skill"].Success && m.Groups["skill"].Value == "Shadow Slash");
            Assert.IsFalse(m.Groups["damage"].Success);
            Assert.IsFalse(m.Groups["HPDrain"].Success);
            Assert.IsTrue(m.Groups["debuff"].Success && m.Groups["debuff"].Value == "Shadow");
        }*/
        

        [TestMethod]
        public void LogParse_LogRead_incomingdamage2_targetdebuff_knockback()
        {
            string testLine = "15:11:33.635|FF|Blocked Sajifi&apos;s Elite Beastmaster&apos;s Searing Strike but received 326 damage and Knockback.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "Sajifi's Elite Beastmaster");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 326);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Searing Strike");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "You");
        }
        
        [TestMethod]
        public void LogParse_LogRead_incomingdamage2_counter()
        {
            string testLine = "15:11:33.635|FF|PLAYER NAME countered Cobalt Widow&apos;s Frost Fury but received 216 damage.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "Cobalt Widow");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 216);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Frost Fury");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "PLAYER NAME");            
        }

        [TestMethod]
        public void LogParse_LogRead_incomingdamage2_receiving()
        {
            string testLine = "15:11:33.635|FF|Engineer Corps Defense Captain partially blocked Breeze receiving 155 damage and 31 HP drain.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 2);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "You");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 155);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Breeze");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Engineer Corps Defense Captain");

            Assert.AreEqual(dependency.CombatActions[1].Attacker, "You");
            Assert.AreEqual(dependency.CombatActions[1].Critical, false);
            Assert.AreEqual(dependency.CombatActions[1].Damage.Number, 31);
            Assert.AreEqual(dependency.CombatActions[1].Special, "Drain");
            Assert.AreEqual(dependency.CombatActions[1].SwingType, (int)SwingTypeEnum.Healing);
            Assert.AreEqual(dependency.CombatActions[1].theAttackType, "Breeze");
            Assert.AreEqual(dependency.CombatActions[1].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[1].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[1].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[1].Victim, "You");            
        }

        [TestMethod]
        public void LogParse_LogRead_incomingdamage3_damage_knockback()
        {
            string testLine = "15:11:33.635|FF|Sochon Gamyung&apos;s Fury inflicted 3563 damage and Knockback.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "Sochon Gamyung");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 3563);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Fury");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "You");
        }
        /*
        // TODO: this is not properly handled - do not know player Blazing Palm is assigned to.
        [TestMethod]
        public void LogParse_LogRead_incomingdamage3_damage_target()
        {
            string testLine = "15:11:33.635|FF|Blazing Palm&apos;s Lasting Effects inflicted 109 damage to Sochon Gamyung.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

        Assert.IsTrue(m.Groups["actor"].Success && m.Groups["actor"].Value == "Blazing Palm");
        Assert.IsTrue(m.Groups["skill"].Success && m.Groups["skill"].Value == "Lasting Effects");
        Assert.IsTrue(m.Groups["damage"].Success && m.Groups["damage"].Value == "109");
        Assert.IsFalse(m.Groups["debuff"].Success);
        Assert.IsTrue(m.Groups["target"].Success && m.Groups["target"].Value == "Sochon Gamyung");

        // todo: add test case for target
        }
        */
        /*
        // TODO: Debuffs are not handled.
        [TestMethod]
        public void LogParse_LogRead_incomingdamage3_nodamage()
        {
            string testLine = "15:11:33.635|FF|Sochon Gamyung&apos;s Stone Shield inflicted Stun.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.IsTrue(m.Groups["actor"].Success && m.Groups["actor"].Value == "Sochon Gamyung");
            Assert.IsTrue(m.Groups["skill"].Success && m.Groups["skill"].Value == "Stone Shield");
            Assert.IsFalse(m.Groups["damage"].Success);
            Assert.IsTrue(m.Groups["debuff"].Success && m.Groups["debuff"].Value == "Stun");
            Assert.IsFalse(m.Groups["target"].Success);
        }*/

        [TestMethod]
        public void LogParse_LogRead_yourdamage_complex()
        {
            string testLine = "15:11:33.635|FF|Frost Fury critically hit Silver Deva for 986 damage, draining 591 HP and 1 Focus.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 2);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "You");
            Assert.AreEqual(dependency.CombatActions[0].Critical, true);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 986);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Frost Fury");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Silver Deva");


            Assert.AreEqual(dependency.CombatActions[1].Attacker, "You");
            Assert.AreEqual(dependency.CombatActions[1].Critical, false);
            Assert.AreEqual(dependency.CombatActions[1].Damage.Number, 591);
            Assert.AreEqual(dependency.CombatActions[1].Special, "Drain");
            Assert.AreEqual(dependency.CombatActions[1].SwingType, (int)SwingTypeEnum.Healing);
            Assert.AreEqual(dependency.CombatActions[1].theAttackType, "Frost Fury");
            Assert.AreEqual(dependency.CombatActions[1].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[1].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[1].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[1].Victim, "You");            
        }

        [TestMethod]
        public void LogParse_LogRead_yourdamage_damage_hpdrain()
        {
            string testLine = "15:11:33.635|FF|Frost Fury hit Junghado for 545 damage and drained 163 HP.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 2);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "You");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage.Number, 545);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Frost Fury");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Junghado");

            Assert.AreEqual(dependency.CombatActions[1].Attacker, "You");
            Assert.AreEqual(dependency.CombatActions[1].Critical, false);
            Assert.AreEqual(dependency.CombatActions[1].Damage.Number, 163);
            Assert.AreEqual(dependency.CombatActions[1].Special, "Drain");
            Assert.AreEqual(dependency.CombatActions[1].SwingType, (int)SwingTypeEnum.Healing);
            Assert.AreEqual(dependency.CombatActions[1].theAttackType, "Frost Fury");
            Assert.AreEqual(dependency.CombatActions[1].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[1].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[1].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[1].Victim, "You");
        }
        /*
                [TestMethod]
                public void LogParse_LogRead_evade_simple()
                {
                    string testLine = "15:11:33.635|FF|PLAYER NAME evaded Scorching Arrow of the Hills.";

                    BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

                    Assert.IsTrue(m.Groups["target"].Success && m.Groups["target"].Value == "PLAYER NAME");
                    Assert.IsTrue(m.Groups["skill"].Success && m.Groups["skill"].Value == "Scorching Arrow of the Hills");
                }*/
        
        [TestMethod]
        public void LogParse_LogRead_defeat_skill()
        {
            string testLine = "15:11:33.635|FF|Chuluun the Strong was defeated by PLAYER NAME&apos;s Sunder.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "PLAYER NAME");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage, Dnum.Death);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Sunder");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "Chuluun the Strong");
        }

        [TestMethod]
        public void LogParse_LogRead_defeat_neardeath()
        {
            string testLine = "15:11:33.635|FF|You were rendered near death by Stalker Jiangshi&apos;s Furious Flurry.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "Stalker Jiangshi");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage, Dnum.Death);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Furious Flurry");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "You");            
        }

        [TestMethod]
        public void LogParse_LogRead_defeat_killed()
        {
            string testLine = "15:11:33.635|FF|You were killed by Ape King Ogong&apos;s Roar.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

            Assert.AreEqual(dependency.CombatActions.Count, 1);
            Assert.AreEqual(dependency.CombatActions[0].Attacker, "Ape King Ogong");
            Assert.AreEqual(dependency.CombatActions[0].Critical, false);
            Assert.AreEqual(dependency.CombatActions[0].Damage, Dnum.Death);
            Assert.AreEqual(dependency.CombatActions[0].Special, "");
            Assert.AreEqual(dependency.CombatActions[0].SwingType, (int)SwingTypeEnum.NonMelee);
            Assert.AreEqual(dependency.CombatActions[0].theAttackType, "Roar");
            Assert.AreEqual(dependency.CombatActions[0].theDamageType, "");
            Assert.AreEqual(dependency.CombatActions[0].Time, DateTime.Parse("15:11:33.635"));
            Assert.AreEqual(dependency.CombatActions[0].TimeSorter, 100000);
            Assert.AreEqual(dependency.CombatActions[0].Victim, "You");
        }

        /* TODO: No actor on heal
        [TestMethod]
        public void LogParse_LogRead_heal_simple()
        {
            string testLine = "15:11:33.635|FF|PLAYERNAME recovered 813 HP with Doom &apos;n&apos; Bloom.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

        Assert.IsTrue(m.Groups["target"].Success && m.Groups["target"].Value == "PLAYERNAME");
        Assert.IsTrue(m.Groups["HPAmount"].Success && m.Groups["HPAmount"].Value == "813");
        Assert.IsFalse(m.Groups["FocusAmount"].Success);
        Assert.IsTrue(m.Groups["skill"].Success && m.Groups["skill"].Value == "Doom &apos;n&apos; Bloom");
        }
        */
        /*
        TODO: No actor on the heal
        [TestMethod]
        public void LogParse_LogRead_heal_you()
        {
            string testLine = "15:11:33.635|FF|Recovered 1125 HP from Doom &apos;n&apos; Bloom.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

        Assert.IsFalse(m.Groups["target"].Success);
        Assert.IsTrue(m.Groups["HPAmount"].Success && m.Groups["HPAmount"].Value == "1125");
        Assert.IsFalse(m.Groups["FocusAmount"].Success);
        Assert.IsTrue(m.Groups["skill"].Success && m.Groups["skill"].Value == "Doom &apos;n&apos; Bloom");
        }*/

        /*
        // TODO: Not implemented

        [TestMethod]
        public void LogParse_LogRead_heal_focus()
        {
        string testLine = "15:11:33.635|FF|PLAYER NAME recovered 2 Focus from Grab.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

        Assert.IsTrue(m.Groups["target"].Success && m.Groups["target"].Value == "PLAYER NAME");
        Assert.IsFalse(m.Groups["HPAmount"].Success);
        Assert.IsTrue(m.Groups["FocusAmount"].Success && m.Groups["FocusAmount"].Value == "2");
        Assert.IsTrue(m.Groups["skill"].Success && m.Groups["skill"].Value == "Grab");
        }
        */
        /*
        // TODO: Not implemented
        [TestMethod]
        public void LogParse_LogRead_buff_simple()
        {
        string testLine = "15:11:33.635|FF|Critical Bonus is now active.";

            BNS_ACT_Plugin.LogParse.BeforeLogLineRead(false, new LogLineEventArgs(testLine, 0, DateTime.Now, "", false));

        Assert.IsTrue(m.Groups["skill"].Success && m.Groups["skill"].Value == "Critical Bonus");
        }
        */
    }
}
