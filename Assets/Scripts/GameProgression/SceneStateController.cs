using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateController : GlobalSingleInstanceMonoBehaviour<SceneStateController>
{
    private bool _gameHandlingDeath = false;
    private bool _gameSaved = false;

    string _gameStateJson = string.Empty;
    string _mouseReceiverJson = string.Empty; // doing this so we know it matches with the players mvmnt controller
    List<CharacterSaveData> _characterSaveData = new();

    private void Update()
    {
        var playerCharacterInfo = CharacterInfoBB.Instance.GetPlayerCharacterInfo();
        if (playerCharacterInfo == null)
            return;

        if (!_gameHandlingDeath && playerCharacterInfo.IsDead)
        {
            _gameHandlingDeath = true;
            StartCoroutine(OnDeathReload());
        }
    }    

    public void SaveGame()
    {
        Debug.Log("Saving game");

        _gameStateJson = JsonUtility.ToJson(GameState.Instance);
        _mouseReceiverJson = JsonUtility.ToJson(MouseReceiver.Instance);

        if (!_characterSaveData.Any())
        {
            var allCharacters = CharacterInfoBB.Instance.GetAll().ToList();
            _characterSaveData = allCharacters.Select(x => new CharacterSaveData(x.gameObject)).ToList();
        }

        foreach (var characterSaveData in _characterSaveData)
        {
            characterSaveData.Save();
        }

        _gameSaved = true;
    }

    public void LoadGame()
    {
        if (!_gameSaved)
            return;

        Debug.Log("Loading game");

        JsonUtility.FromJsonOverwrite(_gameStateJson, GameState.Instance);
        JsonUtility.FromJsonOverwrite(_mouseReceiverJson, MouseReceiver.Instance);

        foreach (var characterSaveData in _characterSaveData)
        {
            characterSaveData.Load();
        }
    }    

    public void RestartGame()
    {
        Debug.Log("Game restarting");
        SceneManager.LoadScene("TheManor");
    }

    private IEnumerator OnDeathReload()
    {
        var load = _gameSaved;

        yield return new WaitForSeconds(3f);

        yield return FadeToBlackController.Instance.FadeToBlackRoutine(3f);

        if (load)
        {
            LoadGame();
            yield return new WaitForSeconds(1f);

            yield return FadeToBlackController.Instance.FadeFromBlackRoutine(3f);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            RestartGame();
        }

        _gameHandlingDeath = false;
    }

    private class CharacterSaveData
    {
        private GameObject _characterGameObject;

        Vector3 _position;
        Quaternion _rotation;

        string _characterInfoJson;
        string _mvmntControllerJson;

        string _npcBrainJson;
        string _characterSecretKnowledgeJson;
        string _lookerJson;
        
        string _playerControllerJson;
        string _playerStatsJson;



        public CharacterSaveData(GameObject characterGameObject)
        {
            _characterGameObject = characterGameObject;
        }

        public void Save()
        {
            _position = _characterGameObject.transform.position;
            _rotation = _characterGameObject.transform.rotation;
            
            _characterInfoJson = ToJson<CharacterInfo>();
            _mvmntControllerJson = ToJson<MvmntController>();

            if (_characterGameObject.transform.IsNpc())
            {
                _npcBrainJson = ToJson<NpcBrain>();
                _characterSecretKnowledgeJson = ToJson<CharacterSecretKnowledge>();
                _lookerJson = ToJson<Looker>();
            }
            else if (_characterGameObject.transform.IsPlayer())
            {
                _playerControllerJson = ToJson<PlayerController>();
                _playerStatsJson = ToJson<PlayerStats>();
            }
        }

        public void Load()
        {
            _characterGameObject.transform.position = _position;
            _characterGameObject.transform.rotation = _rotation;

            WriteJson<CharacterInfo>(_characterInfoJson);
            WriteJson<MvmntController>(_mvmntControllerJson);


            if (_characterGameObject.transform.IsNpc())
            {
                WriteJson<NpcBrain>(_npcBrainJson);
                WriteJson<CharacterSecretKnowledge>(_characterSecretKnowledgeJson);
                WriteJson<Looker>(_lookerJson);

                _characterGameObject.GetComponent<NpcBrain>().ReEvaluateTree();
            }
            else if (_characterGameObject.transform.IsPlayer())
            {
                WriteJson<PlayerController>(_playerControllerJson);
                WriteJson<PlayerStats>(_playerStatsJson);
            }

            _characterGameObject.GetComponent<CharacterInfo>().ReturnToLife();
        }        

        private string ToJson<T>() where T : MonoBehaviour
        {
            return JsonUtility.ToJson(_characterGameObject.GetComponent<T>());
        }

        private void WriteJson<T>(string json) where T : MonoBehaviour
        {
            var monoBehaviour = _characterGameObject.GetComponent<T>();
            JsonUtility.FromJsonOverwrite(json, monoBehaviour);
        }
    }
}