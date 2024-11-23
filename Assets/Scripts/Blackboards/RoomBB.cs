using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public void CharacterLeftRoom(CharacterID characterID, RoomID roomID)
    {
        if (!_characterLocation.ContainsKey(characterID))
            _characterLocation.Add(characterID, RoomID.Unknown);

        if (_characterLocation[characterID] == roomID)
            _characterLocation[characterID] = RoomID.Unknown;
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

        var otherRoomsRandomize = _existingRooms.Where(x => x.ID != currentRoomID).Randomize();

        if (!otherRoomsRandomize.Any())
            return null;

        var maxLoops = 10;
        for (var i = 0; i < maxLoops; i++)
        {
            foreach (var room in otherRoomsRandomize)
            {
                var roomOccupancy = _characterLocation.Count(x => x.Value == room.ID);
                if (room.RandomRoomChance(roomOccupancy))
                    return room;
            }
        }

        Debug.LogWarning("Random chance to find a room never succeeded. Maybe social score should be higher?");
        return null;
    }
}