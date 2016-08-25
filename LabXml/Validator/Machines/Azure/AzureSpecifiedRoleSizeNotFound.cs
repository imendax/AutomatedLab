﻿using System.Collections.Generic;
using System.Linq;

namespace AutomatedLab
{
    /// <summary>
    /// This validator looks for duplicate machine names inside a lab.
    /// </summary>
    public class AzureSpecifiedRoleSizeNotFound : LabValidator, IValidate
    {
        public AzureSpecifiedRoleSizeNotFound()
        {
            messageContainer = RunValidation();
        }

        public override IEnumerable<ValidationMessage> Validate()
        {
            if (lab.AzureSettings == null)
                yield break;

            var roleSizeLables = lab.AzureSettings.RoleSizes.Select(r => r.RoleSizeLabel);

            var machinesWithUnknownRoleSizes = machines
                .Where(machine => machine.HostType == VirtualizationHost.Azure && machine.AzureProperties.ContainsKey("RoleSize"))
                .Where(machine => !roleSizeLables.Contains(machine.AzureProperties["RoleSize"]));

            foreach (var machine in machinesWithUnknownRoleSizes)
            {
                yield return new ValidationMessage()
                {
                    Message = string.Format("The specified role {0} does not exist in Azure", machine.AzureProperties["RoleSize"]),
                    Type = MessageType.Error,
                    TargetObject = machine.Name
                };
            }
        }
    }
}