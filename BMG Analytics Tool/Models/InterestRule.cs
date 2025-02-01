﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMG_Analytics_Tool.Models
{
    public class InterestRule
    {
        public DateTime Date { get; }
        public string RuleId { get; }
        public decimal Rate { get; }

        public InterestRule(DateTime date, string ruleId, decimal rate)
        {
            Date = date;
            RuleId = ruleId;
            Rate = rate;
        }
    }
}
