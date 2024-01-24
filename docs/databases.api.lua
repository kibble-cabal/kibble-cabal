---@diagnostic disable: unused-local, missing-return

---@class AbilityDB
---@field registered_abilities Ability[]
---@field register fun(resource: Ability)
---@field unregister fun(resource: Ability)
---@field find fun(identifier: string): Ability | nil

---Gets the ability database instance.
---@return AbilityDB
function GetAbilityDB() end

---@class ActionDB
---@field registered_actions ActionMenuItem[]
---@field register fun(resource: ActionMenuItem)
---@field unregister fun(resource: ActionMenuItem)
---@field find_by_menu fun(menu_identifier: string): ActionMenuItem[]

---Gets the action database instance.
---@return ActionDB
function GetActionDB() end

---@class AttributeDB
---@field registered_attributes Ability[]
---@field register fun(resource: Attribute)
---@field unregister fun(resource: Attribute)
---@field find fun(identifier: string): Attribute | nil

---Gets the attribute database instance.
---@return AttributeDB
function GetAttributeDB() end

---@class EffectDB
---@field registered_effects Effect[]
---@field register fun(resource: Effect)
---@field unregister fun(resorce: Effect)
---@field find fun(identifier: string): Effect | nil

---Gets the effect database instance.
---@return EffectDB
function GetEffectDB() end

---@class TagDB
---@field registered_tags Tag[]
---@field register fun(resource: Tag)
---@field unregister fun(resource: Tag)
---@field find fun(identifier: string): Tag | nil

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
