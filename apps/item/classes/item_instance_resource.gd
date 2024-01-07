class_name ItemInstanceResource extends ModdableResource

@export var item: ItemResource
@export var creation_time: int

@export_category("Additional data")

## Only applicable if [member item] has [member consumable_resources].
## [br]May split this into separate resource later...
@export var uses: int = 0
