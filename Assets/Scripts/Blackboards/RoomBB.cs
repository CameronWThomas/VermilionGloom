using System;
using System.Collections.Generic;
using System.Linq;

public class RoomBB : GlobalSingleInstanceMonoBehaviour<RoomBB>
{
    private Dictionary<CharacterID, RoomID> _characterLocation = new();
    private List<Room> _existingRooms = new();

    public void Register(Room room)
    {
        if (_existingRooms.Contains(room) || room.ID is RoomID.Unknown)
            return;

        _existingRooms.Add(room);
    }

    public void UpdateCharacterLocation(CharacterID characterID, RoomID roomID)
    {
        if (!_characterLocation.ContainsKey(characterID))
            _characterLocation.Add(characterID, RoomID.Unknown);

        _characterLocation[characterID] = roomID;
    }

    public RoomID GetCharacterRoomID(CharacterID characterID) => _characterLocation.ContainsKey(characterID) ? _characterLocation[characterID] : RoomID.Unknown;

    public IEnumerable<CharacterID> GetCharactersInMyRoom(CharacterID characterID)
    { 
        var roomId = _characterLocation.ContainsKey(characterID) ? _characterLocation[characterID] : RoomID.Unknown;
        if (roomId is RoomID.Unknown)
            yield break;

        foreach (var otherCharacterID in _characterLocation.Where(x => x.Value == roomId && x.Key != characterID))
            yield return otherCharacterID.Key;
    }
    
    public Room GetCharacterRoom(CharacterID characterID)
    {
        var roomID = GetCharacterRoomID(characterID);
        return _existingRooms.FirstOrDefault(x => x.ID == roomID);
    }

    public Room GetRandomOtherRoom(CharacterID characterID)
    {
        var currentRoomID = GetCharacterRoomID(characterID);
        return _existingRooms.Where(x => x.ID != currentRoomID).Randomize().First();
    }
}