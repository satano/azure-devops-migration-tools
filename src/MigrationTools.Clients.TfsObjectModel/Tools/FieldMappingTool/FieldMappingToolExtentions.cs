using System;
using System.Collections.Generic;
using System.Linq;
using MigrationTools.FieldMaps.AzureDevops.ObjectModel;
using MigrationTools.Tools.Infrastructure;
using MigrationTools.Tools.Interfaces;

namespace MigrationTools.Tools
{
    internal static class FieldMappingToolExtentions
    {
        public static IEnumerable<FieldValueMap> GetFieldValueMaps(
            this IFieldMappingTool fieldMappingTool,
            string targetWitName,
            string sourceFieldReferenceName,
            string targetFieldReferenceName)
        {
            if (fieldMappingTool.Items.TryGetValue(targetWitName, out List<IFieldMap>? fieldMaps))
            {
                return fieldMaps
                    .Where(fm => fm is FieldValueMap)
                    .Cast<FieldValueMap>()
                    .Where(fvm => sourceFieldReferenceName.Equals(fvm.Config.sourceField, StringComparison.OrdinalIgnoreCase)
                        && targetFieldReferenceName.Equals(fvm.Config.targetField, StringComparison.OrdinalIgnoreCase));
            }
            return [];
        }

        public static Dictionary<string, string> GetFieldValueMappings(
            this IFieldMappingTool fieldMappingTool,
            string targetWitName,
            string sourceFieldReferenceName,
            string targetFieldReferenceName)
        {
            Dictionary<string, string> result = new(StringComparer.OrdinalIgnoreCase);

            IEnumerable<FieldValueMap> fieldValueMaps = fieldMappingTool
                .GetFieldValueMaps(targetWitName, sourceFieldReferenceName, targetFieldReferenceName);
            foreach (FieldValueMap fieldValueMap in fieldValueMaps)
            {
                foreach (KeyValuePair<string, string> map in fieldValueMap.Config.valueMapping)
                {
                    string sourceValue = map.Key;
                    string targetValue = map.Value;
                    if (result.TryGetValue(sourceValue, out string existingTargetValue)
                        && !existingTargetValue.Equals(targetValue, StringComparison.OrdinalIgnoreCase))
                    {
                        string msg = $"Conflict in field value mapping for '{sourceFieldReferenceName}' to '{targetFieldReferenceName}' in '{targetWitName}': "
                            + $"Value '{sourceValue}' maps to both '{existingTargetValue}' and '{targetValue}'.";
                        throw new Exception(msg);
                    }
                    result[sourceValue] = targetValue;
                }
            }
            return result;
        }
    }
}
