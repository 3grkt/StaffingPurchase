using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using StaffingPurchase.Web.Validators;

namespace StaffingPurchase.Web.Models.Configurations
{
    [Validator(typeof(ConfigurationValidator))]
    public class ConfigurationModel : ViewModelBase
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class PolicyConfigurationModel
    {
        public IList<ConfigurationModel> AllConfigurations { get; set; }
        public ConfigurationModel PolicyDocument { get; set; }
    }
}
