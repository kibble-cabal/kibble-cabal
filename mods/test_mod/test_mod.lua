function OnLocationEntered(location)
    Log.start_section("TestMod", "Running test mod...")
    Log.bullet("Detected location entrance!")
    local current_location = GetLocationSystem().current_location
    if current_location ~= nil then
        Log.bullet("Current location is: " .. current_location.name)
    else
        Log.warning("Warning: Island location not found!")
    end
    Log.end_section("TestMod")
end

-- Test out ZIPReader

Log.from("TestMod", "Current mod info:")
local reader = ZIPReader()
reader.open(GetCurrentMod().zip_path)
print(reader.read_string("test_mod/test_mod.mod.json"))
reader.close()
