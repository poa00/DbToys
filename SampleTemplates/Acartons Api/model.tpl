{{~ # Required: Defines the output filename here. ~}}
{{~ filename = (table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular) + ".cs" ~}} 
{{~ classname = table.clean_name | regex.replace "^[a-zA-Z0-9]+_" "" | string.to_pascal_case | string.to_singular ~}}
{{~ # Samples link: https://github.com/NeilQ/DbToys/blob/master/SampleTemplates ~}}
{{~ # How to write template: https://github.com/NeilQ/DbToys/wiki/Code-template-instruction ~}}
{{~ 
cols= table.columns | array.map "name" 
ignoredCols=["id","add_user","add_time","update_time","update_user","marked_for_delete","delete_time","delete_user"]
hasDeleteFields= (cols | array.contains "delete_user") && (cols | array.contains "delete_time")
hasUpdateFields= (cols | array.contains "update_user") && (cols | array.contains "update_time")
hasAddFields= (cols | array.contains "add_user") && (cols | array.contains "add_time")

func get_base_type()
    if hasDeleteFields && hasAddFields && hasUpdateFields && hasAddFields
        ret "FullAuditedEntityBase"
    end
    if hasAddFields && hasUpdateFields 
        ret "AuditedEntityBase"
    end
    if hasAddFields 
        ret "CreateAuditedEntityBase"
    end
end
~}}
using System.ComponentModel.DataAnnotations.Schema;
using Acartons.Core.Entities;

namespace Acartons.Domain.{{ classname | string.to_plural }};

[Table("{{ table.clean_name }}")]
public class {{ classname }} : {{get_base_type}}
{ 
    {{~ for col in table.columns ~}}   
      {{~ if !(ignoredCols | array.contains col.name) ~}}
        {{~ if col.description && col.description!="" ~}}
    /// <summary> 
    /// {{col.description}}
    /// </summary>
        {{~ end ~}}
    public {{ col.db_type | get_property_type_of_pgsql }} {{ col.property_name | string.to_pascal_case }} { get; set; }

      {{~ end ~}}
    {{~ end ~}}
}   
