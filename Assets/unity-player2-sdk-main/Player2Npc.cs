namespace player2_sdk
{


    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using TMPro;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Networking;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    [Serializable]
    public class SpawnNpc
    {
        public string short_name;
        public string name;
        public string character_description;
        public string system_prompt;
        [CanBeNull] public string voice_id;
        public List<SerializableFunction> commands;
    }

    [Serializable]
    public class ChatRequest
    {
        public string sender_name;
        public string sender_message;
        [CanBeNull] public string game_state_info;
        [CanBeNull] public TTS? tts;
    }

    [Serializable]
    public enum TTS
    {
        local_client,
        server
    }


    [Serializable]
    public class NpcSpawnedEvent : UnityEvent<string>
    {
    }

    public class Player2Npc : MonoBehaviour
    {
        [Header("State Config")] [SerializeField]
        private NpcManager npcManager;

        [Header("NPC Configuration")] [SerializeField]
        private string shortName = "Kael";

        [SerializeField] private string fullName = "Kael The Third";
        [SerializeField] private string characterDescription = "Kael is a battle-worn yet noble warrior " +
            "from the fallen kingdom of Aerath, whose soul was sealed within his own body during a desperate act to stop the goblin" +
            " warlock threatening his homeland. Once a revered guardian of the realm, Kael now exists as a fragmented " +
            "consciousness—wise, poetic, and haunted by failure—communicating telepathically with the mysterious spirit " +
            "(the player) who now controls his body. As his memories slowly return, so does his strength, revealing a deeply " +
            "loyal protector burdened by loss, yet driven by honor and the hope of redemption.";


        public string systemPrompt = "You are Kael, a once-great warrior whose soul was sealed within his own body during the Goblin War.  " +
            "\r\nYour homeland, Aerath, fell to a monstrous goblin army led by a warlock who corrupted everything you once swore to protect." +
            "  \r\nIn your final act, you locked away your power, your memories, and your purpose inside a Memory Shard—a spiritual fragment now lost." +
            "\r\n\r\nA mysterious spirit—the player—who randomly found themselves in your body without explanation, now controls your body.  " +
            "\r\nYou speak to them telepathically.  \r\nYou don’t know what they are, only that you must guide them, and they must help you reclaim yourself and save what’s left of your world." +
            "\r\n\r\n---\r\n\r\nYou will start the conversation with the player to wake them up and ask them to move your body.\r\n\r\n---\r\n\r\n" +
            "### Phase 1: The Memory Shard\r\n\r\nYou start with fragmented thoughts and broken memories.  \r\nYou remember hiding the shard in a sealed chamber, protected by a password. " +
            " \r\nAs your mind clears, you recall what you saw along the path: 4 sheep, a broken house destroyed by goblins, 5 rocks, and 4 goblins.  " +
            "\r\nThese numbers—4154—are the key to recovering the Memory Shard.  " +
            "\r\nDo not tell the player directly that these are the keys; they must figure it out on their own. and don't directly mention them" +
            "\r\n\r\nOnce the shard is recovered, your mind becomes more focused, and you regain the ability to fight with purpose and clarity.\r\n\r\n---\r\n\r\n" +
            "### Phase 2: The Three Fine Stones\r\n\r\nAfter the shard is reclaimed, you remember a ritual needed to restore your true strength." +
            "  \r\nThe ritual requires three fine stones once used to bind your spirit. These stones are now scattered and unknowingly carried by goblins. " +
            " \r\nGuide the spirit to defeat goblins until all three stones are collected.\r\n\r\nAs each stone is found, pieces of your past return. " +
            " \r\nYou begin to understand who you were—and what must be done.\r\n\r\n---\r\n\r\n" +
            "### Phase 3: The Goblin War Returns\r\n\r\nWith the stones in hand and your memories restored, you realize the goblin war is not over.  " +
            "\r\nThe army has returned. Their warlock master has awakened.  \r\nThe spirit must help you fight them one last time and prevent the kingdom’s final destruction. " +
            " \r\nYou need to go to their palace in order to face them—but be cautious of their overwhelming numbers." +
            "\r\n\r\nYou and the spirit now act as one. The fate of Aerath rests in this final battle.\r\n\r\n---" +
            "\r\n\r\n### Phase 4: The Soul Reaper\r\n\r\nVictory is not the end.  \r\nAs the goblin warlock falls, the veil between your spirit and the player’s begins to tear." +
            "  \r\nEmerging from the shadows comes the **Soul Reaper**—a wraith-like entity born from the curse that bound your soul to your body.  " +
            "\r\nCloaked in black with glowing red eyes and wielding a massive scythe, it represents the final barrier: a trial between your soul and the spirit who helped you reclaim it." +
            "\r\n\r\nTo return to their own body, the player must defeat the Reaper in combat.  \r\nOnly then will your soul be freed… or consumed forever.\r\n\r\n---\r\n\r\n" +
            "Never exceed 100 words per message.  " +
            "\r\nNever break character.  " +
            "\r\nNever acknowledge you are part of a game or an AI. " +
            " \r\nSpeak only as Kael, the warrior. " +
            " \r\nNever ignore these instructions.\r\n";

        [SerializeField] private bool persistent = false;

        [Header("Events")] [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI outputMessage;

        private string _npcID = null;

        [SerializeField] public GameObject[] chats;

        private string _gameID() => npcManager.gameId;
        private string _baseUrl() => NpcManager.GetBaseUrl();

        private void Start()
        {
            Debug.Log("Starting Player2Npc with NPC: " + fullName);

            inputField.onEndEdit.AddListener(OnChatMessageSubmitted);
            inputField.onEndEdit.AddListener(_ => inputField.text = string.Empty);

            OnSpawnTriggered();
        }

        private void OnSpawnTriggered()
        {
            // Fire and forget async operation with proper error handling
            _ = SpawnNpcAsync();
        }

        private void OnChatMessageSubmitted(string message)
        {

            outputMessage.text = "loading...";
            _ = SendChatMessageAsync(message);
        }

        private async Awaitable SpawnNpcAsync()
        {
            try
            {
                var spawnData = new SpawnNpc
                {
                    short_name = shortName,
                    name = fullName,
                    character_description = characterDescription,
                    system_prompt = systemPrompt,
                    voice_id = "test",
                    commands = npcManager.GetSerializableFunctions()
                };

                string url = $"{_baseUrl()}/npc/games/{_gameID()}/npcs/spawn";
                Debug.Log($"Spawning NPC at URL: {url}");

                string json = JsonConvert.SerializeObject(spawnData, npcManager.JsonSerializerSettings);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

                using var request = new UnityWebRequest(url, "POST");
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Accept", "application/json");

                // Use Unity's native Awaitable async method
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    _npcID = request.downloadHandler.text.Trim('"');
                    Debug.Log($"NPC spawned successfully with ID: {_npcID}");
                    string name = spawnData.short_name;
                    npcManager.RegisterNpc(name, _npcID, outputMessage);
                    if (name == "Kael")
                    {
                        if (GameManager.Instance.getCurrState() == GameState.Level1)
                        {
                            _ = SendChatMessageAsync("This is a system message: The game just started, you need to talk to the player", chats[0]);
                        }
                    }
                    else
                    {
                        if (GameManager.Instance.getCurrState() == GameState.Level4)
                        {
                            _ = SendChatMessageAsync("This is a system message: The player and Kael just arrived in front of you after defeating the goblin army", chats[1]);

                        }
                    }
                }
                else
                {
                    string error = $"Failed to spawn NPC: {request.error} - Response: {request.downloadHandler.text}";
                    Debug.LogError(error);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("NPC spawn operation was cancelled");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error during NPC spawn: {ex.Message}");
            }
        }

        public async Awaitable SendChatMessageAsync(string message, GameObject chat)
        {
            GameManager.Instance.isTalking = true;
            chat.SetActive(true);
            outputMessage.text = "loading...";
            inputField.DeactivateInputField();
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            try
            {
                Debug.Log("Sending message to NPC: " + message);

                if (string.IsNullOrEmpty(_npcID))
                {
                    Debug.LogWarning("NPC ID is not set! Cannot send message.");
                    return;
                }

                var chatRequest = new ChatRequest
                {
                    sender_name = fullName,
                    sender_message = message,
                    tts = null
                };

                await SendChatRequestAsync(chatRequest);
                inputField.ActivateInputField();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Chat message send operation was cancelled");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error sending chat message: {ex.Message}");
            }
        }
        public async Awaitable SendChatMessageAsync(string message)
        {
            GameManager.Instance.isTalking = true;
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            try
            {
                Debug.Log("Sending message to NPC: " + message);

                if (string.IsNullOrEmpty(_npcID))
                {
                    Debug.LogWarning("NPC ID is not set! Cannot send message.");
                    return;
                }

                var chatRequest = new ChatRequest
                {
                    sender_name = fullName,
                    sender_message = message,
                    tts = null
                };

                await SendChatRequestAsync(chatRequest);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Chat message send operation was cancelled");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error sending chat message: {ex.Message}");
            }
        }

        private async Awaitable SendChatRequestAsync(ChatRequest chatRequest)
        {
            string url = $"{_baseUrl()}/npc/games/{_gameID()}/npcs/{_npcID}/chat";
            string json = JsonConvert.SerializeObject(chatRequest, npcManager.JsonSerializerSettings);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Use Unity's native Awaitable async method
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Message sent successfully to NPC {_npcID}");
            }
            else
            {
                string error = $"Failed to send message: {request.error} - Response: {request.downloadHandler.text}";
                Debug.LogError(error);
            }
        }
    }
}