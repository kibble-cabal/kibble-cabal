-- Called every time the in-game clock ticks.
function OnDatetimeTicked()
    Log.log("Date ticked!")
end

local my_feature_enabled = false

-- Called for each installed expansion pack.
-- This example shows how you can handle a dependency on the "core" expansion pack.
function OnExpansionPackRegistered(pack)
    if pack.id == "core" then
        my_feature_enabled = true
    end
end

-- In this example, we change the name of the "dog" animal to "doge".
-- Because dogs were added in "core/pet", this requires the "core" expansion pack.
function main()
    if my_feature_enabled then
        local animal_db = GetAnimalDB()
        local dog = animal_db.find("dog")
        dog.name = "doge"
    end
end

main()
