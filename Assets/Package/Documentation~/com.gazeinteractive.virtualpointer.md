# Virtual Pointer

## Gaze.VirtualPointer.VirtualPointer (Class)

## Properties

### PointerDown

Set as true to simulate a "Button Down"/"Mouse Button Down", in order to start an interaction
Set as false to "Release" the pointer so it can generate a "Click" 

## Methods

### MovePointer(Vector2 movement)

Move the pointer additively by the exactly movement vector magnitude.

#### movement

Vector2 composed by the horizontal and vertical movement.

### SetPointerPosition(Vector2 position)

Sets the pointer position on the world.

#### position

The world position coordinates.
