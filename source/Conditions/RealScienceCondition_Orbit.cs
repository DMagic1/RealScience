﻿using System;
using System.Collections.Generic;

using UnityEngine;

namespace RealScience.Conditions
{
    public class RealScienceCondition_Orbit : RealScienceCondition
    {
        // common properties
        public string conditionType = "Orbit";
        public bool restriction = false;
        public string exclusion = "";
        public float dataRateModifier = 1f;
        public float maximumDataModifier = 1f;
        public float maximumDataBonus = 0f;

        // specific properties
        public string mainBody = "kerbin";
        public float eccentricityMin = 0f;
        public float eccentricityMax = 1f;
        public float apoapsisMin = float.MinValue;
        public float apoapsisMax = float.MaxValue;
        public float periapsisMin = float.MinValue;
        public float periapsisMax = float.MaxValue;
        public float inclinationMin = 0f;
        public float inclinationMax = 180f;
        public float velocityMin = 0f;
        public float velocityMax = float.MaxValue;

        protected string tooltip;

        public override float DataRateModifier
        {
            get { return dataRateModifier; }
        }
        public override float MaximumDataModifier
        {
            get { return maximumDataModifier; }
        }
        public override float MaximumDataBonus
        {
            get { return maximumDataBonus; }
        }
        public override bool IsRestriction
        {
            get { return restriction; }
        }
        public override string Exclusion
        {
            get { return exclusion; }
        }
        public override string Tooltip
        {
            get { return tooltip; }
        }
        public override string Name
        {
            get { return conditionType; }
        }

		public override EvalState Evaluate(Part part, float deltaTime, ExperimentState state)
        {
            bool valid = true;
            double r_ap = part.vessel.orbit.semiMajorAxis * (1 + part.vessel.orbit.eccentricity);
            double r_pe = part.vessel.orbit.semiMajorAxis * (1 - part.vessel.orbit.eccentricity);
            double radius = part.vessel.orbit.referenceBody.Radius;
            double ap = r_ap - radius;
            double pe = r_pe - radius;
            tooltip = "\nOrbit Condition";
            if (restriction)
            {
                if (exclusion.ToLower() == "reset")
                    tooltip += "\nThe following conditions must <b>not</b> be met.  If they are the experiment will be <b>reset</b>.";
                else if (exclusion.ToLower() == "fail")
                    tooltip += "\nThe following conditions must <b>not</b> be met.  If they are, the experiment will <b>fail</b>.";
                else
                    tooltip += "\nThe following conditions must <b>not</b> be met.";
            }
            else
                tooltip += "\nThe following conditions must be met.";
            tooltip += String.Format("\nCraft sphere of influence equal to <b>{0}</b>.  Currently <b>{1}</b>", mainBody, part.vessel.mainBody.ToString().ToLower());
            tooltip += String.Format("\nEccentricity between <b>{0:F2}</b> and <b>{1:F2}</b>.  Currently <b>{2:F2}</b>", eccentricityMin, eccentricityMax, part.vessel.orbit.eccentricity);
            tooltip += String.Format("\nInclination between <b>{0:F2}</b> and <b>{1:F2}</b>.  Currently <b>{2:F2}</b>", inclinationMin, inclinationMax, part.vessel.orbit.inclination);
            tooltip += String.Format("\nApoapsis between <b>{0:F2}</b> and <b>{1:F2}</b>.  Currently <b>{2:F2}</b>", apoapsisMin, apoapsisMax, ap);
            tooltip += String.Format("\nPeriapsis between <b>{0:F2}</b> and <b>{1:F2}</b>.  Currently <b>{2:F2}</b>", periapsisMin, periapsisMax, pe);
            tooltip += String.Format("\nOrbital velocity between <b>{0:F2}</b> and <b>{1:F2}</b>.  Currently <b>{2:F2}</b>", velocityMin, velocityMax, part.vessel.obt_speed);

            if (part.vessel.mainBody.name.ToLower() != mainBody.ToLower())
                valid = false;
            if (part.vessel.orbit.eccentricity < eccentricityMin)
                valid = false;
            if (part.vessel.orbit.eccentricity > eccentricityMax)
                valid = false;
            if (part.vessel.orbit.inclination < inclinationMin)
                valid = false;
            if (part.vessel.orbit.inclination > inclinationMax)
                valid = false;
            if ((float)ap < apoapsisMin)
                valid = false;
            if ((float)ap > apoapsisMax)
                valid = false;
            if ((float)pe < periapsisMin)
                valid = false;
            if ((float)pe > periapsisMax)
                valid = false;
            if (part.vessel.obt_speed < velocityMin)
                valid = false;
            if (part.vessel.obt_speed > velocityMax)
                valid = false;

            if (!restriction)
            {
                if (valid)
                    return EvalState.VALID;
                else
                    return EvalState.INVALID;
            }
            else
            {
                if (!valid)
                    return EvalState.VALID;
                else
                {
                    if (exclusion.ToLower() == "reset")
                        return EvalState.RESET;
                    else if (exclusion.ToLower() == "fail")
                        return EvalState.FAILED;
                    else
                        return EvalState.INVALID;
                }
            }
        }
        public override void Load(ConfigNode node)
        {
            // Load common properties
            if (node.HasValue("conditionType"))
                conditionType = node.GetValue("conditionType");
            if (node.HasValue("restriction"))
            {
                try
                {
                    restriction = bool.Parse(node.GetValue("restriction"));
                }
                catch (FormatException)
                {
                    restriction = false;
                }
            }
            if (node.HasValue("dataRateModifier"))
            {
                try
                {
                    dataRateModifier = float.Parse(node.GetValue("dataRateModifier"));
                }
                catch (FormatException)
                {
                    dataRateModifier = 1f;
                }
            }
            // Load specific properties
            if (node.HasValue("mainBody"))
                mainBody = node.GetValue("mainBody");
            if (node.HasValue("eccentricityMin"))
                eccentricityMin = float.Parse(node.GetValue("eccentricityMin"));
            if (node.HasValue("eccentricityMax"))
                eccentricityMax = float.Parse(node.GetValue("eccentricityMax"));
            if (node.HasValue("apoapsisMin"))
                apoapsisMin = float.Parse(node.GetValue("apoapsisMin"));
            if (node.HasValue("apoapsisMax"))
                apoapsisMax = float.Parse(node.GetValue("apoapsisMax"));
            if (node.HasValue("periapsisMin"))
                periapsisMin = float.Parse(node.GetValue("periapsisMin"));
            if (node.HasValue("periapsisMax"))
                periapsisMax = float.Parse(node.GetValue("periapsisMax"));
            if (node.HasValue("inclinationMin"))
                inclinationMin = float.Parse(node.GetValue("inclinationMin"));
            if (node.HasValue("inclinationMax"))
                inclinationMax = float.Parse(node.GetValue("inclinationMax"));
            if (node.HasValue("velocityMin"))
                velocityMin = float.Parse(node.GetValue("velocityMin"));
            if (node.HasValue("velocityMax"))
                velocityMax = float.Parse(node.GetValue("velocityMax"));

        }
        public override void Save(ConfigNode node)
        {
            // Save common properties
            node.AddValue("conditionType", conditionType);
            node.AddValue("restriction", restriction);
            node.AddValue("dataRateModifier", dataRateModifier);
            // Save specific properties
            node.AddValue("mainBody", mainBody);
            node.AddValue("eccentricityMin", eccentricityMin);
            node.AddValue("eccentricityMax", eccentricityMax);
            node.AddValue("apoapsisMin", apoapsisMin);
            node.AddValue("apoapsisMax", apoapsisMax);
            node.AddValue("periapsisMin", periapsisMin);
            node.AddValue("periapsisMax", periapsisMax);
            node.AddValue("inclinationMin", inclinationMin);
            node.AddValue("inclinationMax", inclinationMax);
            node.AddValue("velocityMin", velocityMin);
            node.AddValue("velocityMax", velocityMax);
        }
    }
}
