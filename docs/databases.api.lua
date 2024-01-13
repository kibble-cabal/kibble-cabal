---@diagnostic disable: unused-local, missing-return

---@class AbilityDB
---@field registered_abilities AAbility[]
---@field registered_stages AAbilityStage[]
---@field register_ability fun(resource: AAbility)
---@field unregister_ability fun(resource: AAbility)
---@field find_ability fun(name: string): AAbility | nil
---@field register_stage fun(resource: AAbilityStage)
---@field unregister_stage fun(resource: AAbilityStage)
---@field find_stage fun(name: string): AAbilityStage | nil

---Gets the ability database instance.
---@return AbilityDB
function GetAbilityDB() end

---@class AttributeDB
---@field registered_attributes AAbility[]
---@field registered_attribute_table_templates AAttributeTableTemplate[]
---@field register_attribute fun(resource: AAttribute)
---@field unregister_attribute fun(resource: AAttribute)
---@field find_attribute fun(name: string): AAttribute | nil
---@field register_attribute_table_template fun(resource: AAttributeTableTemplate)
---@field unregister_attribute_table_template fun(resource: AAttributeTableTemplate)
---@field find_attribute_table_template fun(name: string): AAttributeTableTemplate | nil

---Gets the attribute database instance.
---@return AttributeDB
function GetAttributeDB() end

---@class EffectDB
---@field registered_effects AEffect[]
---@field register fun(resource: AEffect)
---@field unregister fun(resorce: AEffect)
---@field find fun(resource_name: string): AEffect | nil

---Gets the effect database instance.
---@return EffectDB
function GetEffectDB() end

---@class TagDB
---@field registered_tags ATag[]
---@field registered_tag_groups ATagGroup[]
---@field register_tag fun(resource: ATag)
---@field unregister_tag fun(resource: ATag)
---@field find_tag fun(name: string): ATag | nil
---@field register_tag_group fun(resource: ATagGroup)
---@field unregister_tag_group fun(resource: ATagGroup)
---@field find_tag_group fun(name: string): ATagGroup | nil

---Gets the tag database instance.
---@return TagDB
function GetTagDB() end

---@class SubtreeDB
---@field registered_subtrees SubtreeResource[]
---@field register fun(resource: SubtreeResource)
---@field unregister fun(resorce: SubtreeResource)
---@field find_by_key fun(key: string): SubtreeResource | nil

---Gets the subtree database instance.
---@return SubtreeDB
function GetSubtreeDB() end

---@class AnimalDB
---@field registered_animals AnimalResource[]
---@field register fun(resource: AnimalResource)
---@field unregister fun(resorce: AnimalResource)
---@field find fun(name: string): AnimalResource | nil

---Gets the animal database instance.
---@return AnimalDB
function GetAnimalDB() end

---@class ExpansionPackDB
---@field registered_expansion_packs ExpansionPackResource[]
---@field loader ExpansionPackLoader
---@field find fun(name: string): ExpansionPackResource | nil

---Gets the expansion pack database instance.
---@return ExpansionPackDB
function GetExpansionPackDB() end

---@class GameModeDB
---@field registered_game_modes GameModeResource[]
---@field register fun(resource: GameModeResource)
---@field unregister fun(resorce: GameModeResource)
---@field find fun(name: string): GameModeResource | nil

---Gets the game mode database instance.
---@return GameModeDB
function GetGameModeDB() end

---@class ItemDB
---@field registered_items ItemResource[]
---@field register fun(resource: ItemResource)
---@field unregister fun(resorce: ItemResource)
---@field find_by_name fun(name: string): ItemResource | nil
---@field find_by_id fun(id: string): ItemResource | nil

---Gets the item database instance.
---@return ItemDB
function GetItemDB() end

---@class LocationDB
---@field registered_locations LocationResource[]
---@field register fun(resource: LocationResource)
---@field unregister fun(resorce: LocationResource)
---@field find fun(name: string): LocationResource | nil

---Gets the location database instance.
---@return LocationDB
function GetLocationDB() end

---@class ModDB
---@field registered_mods ModResource[]
---@field register fun(resource: ModResource)
---@field unregister fun(resorce: ModResource)
---@field find fun(name: string): ModResource | nil

---Gets the mod database instance.
---@return ModDB
function GetModDB() end

---@class MusicDB
---@field registered_music MusicResource[]
---@field register fun(resource: MusicResource)
---@field unregister fun(resorce: MusicResource)
---@field find fun(name: string): MusicResource | nil

---Gets the music database instance.
---@return MusicDB
function GetMusicDB() end

---@class QuestDB
---@field registered_quests QuestResource[]
---@field register fun(resource: QuestResource)
---@field unregister fun(resorce: QuestResource)
---@field find_by_name fun(name: string): QuestResource | nil
---@field find_by_id fun(name: string): QuestResource | nil

---Gets the quest database instance.
---@return QuestDB
function GetQuestDB() end

---@class SettingDefinitionDB
---@field registered_settings SettingDefinitionResource[]
---@field register fun(resource: SettingDefinitionResource)
---@field unregister fun(resorce: SettingDefinitionResource)
---@field find_by_name fun(name: string): SettingDefinitionResource | nil
---@field find_by_id fun(name: string): SettingDefinitionResource | nil

---Gets the setting definition database instance.
---@return SettingDefinitionDB
function GetSettingDefinitionDB() end

---@class SoundEffectDB
---@field registered_sound_effects AudioStream[]
---@field register fun(resource: AudioStream)
---@field unregister fun(resorce: AudioStream)
---@field find_by_path fun(path: string): AudioStream | nil

---Gets the sound effect database instance.
---@return SoundEffectDB
function GetSoundEffectDB() end
