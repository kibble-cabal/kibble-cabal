class_name ItemPhysicsResource extends ModdableResource

## Represents the object in-world. Can be used instead of [member static_image]
## to add functionality like collision, animation, etc.
@export var scene: PackedScene

## Represents an in-world image. Can be used instead of [member scene] for items that don't need
## collision, animation, etc (e.g. wallpapers).
@export var static_image: Texture2D
