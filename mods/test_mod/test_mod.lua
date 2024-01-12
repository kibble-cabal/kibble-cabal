Log.start_section("TestMod", "Running test mod...")
local current_location = GetLocationSystem().current_location
if current_location ~= nil then
    Log.from("TestMod", "Detected current location: " .. current_location.name)
else
    Log.warning_from("TestMod", "Warning: Island location not found!")
end
Log.end_section("TestMod")
