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

-- Resources

---@class Ability
---@class ActionMenuItem
---@class Attribute
---@class Effect
---@class Tag
---@class SubtreeResource
---@class AnimalResource
---@class ExpansionPackResource
---@class GameModeResource
---@class InventoryResource
---@class ItemResource
---@class LocationResource
---@class ModResource
---@class MusicResource
---@class PetResource
---@class PlayerResource
---@class QuestResource
---@class SaveResource
---@class SettingDefinitionResource
---@class SettingsResource
---@class AudioStream

-- Other

---@class ExpansionPackLoader

-- Static objects

DatetimeHelper = nil
Log = nil
