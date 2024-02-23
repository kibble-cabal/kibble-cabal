public static class Bit
{
    public enum Layer : uint
    {
        L1 = 1 << 0,
        L2 = 1 << 1,
        L3 = 1 << 2,
        L4 = 1 << 3,
        L5 = 1 << 4,
        L6 = 1 << 5,
        L7 = 1 << 6,
        L8 = 1 << 7,
        L9 = 1 << 8,
        L10 = 1 << 9,
        L11 = 1 << 10,
        L12 = 1 << 11,
        L13 = 1 << 12,
        L14 = 1 << 13,
        L15 = 1 << 14,
        L16 = 1 << 15,
        L17 = 1 << 16,
        L18 = 1 << 17,
        L19 = 1 << 18,
        L20 = 1 << 19
    }

    public enum Physics : uint
    {
        World = Layer.L1,
        Players = Layer.L2,
        Pets = Layer.L3,
        Items = Layer.L4,
        Buildings = Layer.L5,

        // UI
        UIDrag = Layer.L10,
        UIDrop = Layer.L11,
        UIPhysicsRay = Layer.L12
    }
}