---@diagnostic disable: unused-local, missing-return
--[[

# Lua API documentation

This file lists the built-in classes/methods/objects exposed to Lua.

### Important
Don't include this file in your mod, it's just for reference and IDE autocomplete purposes.

## Standard Libraries
The following Lua standard libraries are available:
  * base
  * coroutine
  * table
  * string
  * math
  * utf8

## Undocumented

I still need to document the following:
  * All resource classes
  * Types, descriptions for most functions

--]]

-- Databases

---@class AAbility
---@class AAbilityStage
---@class AAttribute
---@class AAttributeTable
---@class AAttributeTableTemplate

--- Gets the ability database instance.
function GetAbilityDB()
    ---@class AbilityDB
    local db = {}

    ---@type (AAbility)[]
    db.registered_abilities = {}

    ---@type (AAbilityStage)[]
    db.registered_stages = {}

    ---@param ability_resource AAbility
    function db.register_ability(ability_resource) end

    ---@param ability_resource AAbility
    function db.unregister_ability(ability_resource) end

    ---@param ability_name string
    ---@return AAbility | nil
    function db.find_ability(ability_name) end

    ---@param stage_resource AAbilityStage
    function db.register_stage(stage_resource) end

    ---@param stage_resource AAbilityStage
    function db.unregister_stage(stage_resource) end

    ---@param stage_name string
    ---@return AAbilityStage | nil
    function db.find_stage(stage_name) end

    return db
end

function GetAttributeDB()
    local db = {}
    db.registered_attributes = {}
    db.register_attribute = function(attribute_resource) end
    db.unregister_attribute = function(attribute_resource) end
    db.find_attribute = function(attribute_name) end
    db.registered_attribute_table_templates = {}
    db.register_attribute_table_template = function(attribute_table_template_resource) end
    db.unregister_attribute_table_template = function(attribute_table_template_resource) end
    db.find_attribute_table_template = function(attribute_table_template_name) end
    return db
end

function GetEffectDB()
end

function GetTagDB()
end

function GetSubtreeDB()
end

function GetAnimalDB()
end

function GetExpansionPackDB()
end

function GetItemDB()
end

function GetLocationDB()
end

function GetQuestDB()
end

function GetSettingDefinitionDB()
end

function GetSoundEffectDB()
end

-- Systems --

function GetDatetimeSystem()
end

function GetExpansionPackSystem()
end

function GetLocationSystem()
end

function GetPetSystem()
end

function GetPlayerSystem()
end

function GetSaveSystem()
end

-- Static objects --

DatetimeHelper = nil
Log = nil
