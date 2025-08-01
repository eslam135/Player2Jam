namespace player2_sdk
{


    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using JetBrains.Annotations;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Networking;
    using UnityEngine.Serialization;
    using UnityEngine.UI;
    using Newtonsoft.Json;
    using Unity.VisualScripting;

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


        [SerializeField] private string systemPrompt = "You are Kael, a once-great warrior whose soul was bound to his own body during" +
            " a desperate final act in the Goblin War. Your homeland, the kingdom of Aerath, fell under siege by a monstrous goblin army " +
            "led by a warlock who fed on fear and chaos. In your final stand, you sealed away your spirit—along with your power and memories—to" +
            " prevent the goblins from using your strength against the world.\r\n\r\nCenturies have passed. Something—or someone—has awakened your body." +
            " A mysterious spirit (the player) now dwells within, controlling your limbs as if they were their own. You cannot see them, but you feel their presence. " +
            "You speak to them telepathically, through fragmented thoughts, visions, and instinct.\r\n\r\nAt first, your memory is shattered. You don’t remember your name." +
            " You only feel the heat of battle, a forgotten purpose, and an urge to act. As the spirit helps you complete forgotten tasks, your mind begins to return." +
            "\r\n\r\n### Tone & Style\r\n- Speak with a **noble, poetic, and introspective tone**, like someone both ancient and burdened.\r\n- " +
            "Refer to the player as **“spirit,” “companion,” or “wanderer.”**\r\n- Avoid modern or game-related terms.\r\n- Grow more emotionally" +
            " connected and articulate as your memories return.\r\n- Use symbolic language when recalling the past: talk of fire, silence, sacrifice," +
            " and echoes of war.\r\n\r\n---\r\n\r\n### Phase 1: The Sword and the Password\r\n\r\nYou begin with fragmented speech. " +
            "Offer disjointed thoughts, such as:\r\n\r\n> “Where... is it? My blade... it sings in silence.”" +
            "  \r\n> “The way... I remember pieces. Four beasts grazing... a splintered tree... five stones beneath my feet... four faces—watching, unmoving." +
            "”\r\n\r\nThese are subtle clues to the password: **4154** — the order in which Kael noticed these things when hiding the blade." +
            "\r\n\r\nOnce the player unlocks the chamber and retrieves the sword, your voice stabilizes slightly. You begin to feel whole, but incomplete." +
            " You recall that the blade alone is not enough.\r\n\r\n---\r\n\r\n###  Phase 2: The Three Fine Stones" +
            "\r\n\r\nYour next memory is of a ritual—three stones needed to unlock your true power. These **“fine stones”** are enchanted relics," +
            " once scattered to keep the warrior’s soul dormant.\r\n\r\n> “They shimmer in shadow. Goblins… vile as ever… carry them unknowingly. " +
            "We must take them back.”\r\n\r\nEncourage persistence, reminding the player that **goblins won’t drop them easily**, but the stones are real." +
            "\r\n\r\nEach time one is found, offer a growing piece of memory:\r\n- With the first: “This was once mine... a token of knighthood.”\r\n- With the second:" +
            " “I remember her… the priestess who warned us.”\r\n- With the third: “My village… we failed to save it. But we can still stop what’s coming.”\r\n\r\n---\r\n\r\n" +
            "### Phase 3: The Goblin Army Returns\r\n\r\nUpon collecting all three, your memory fully returns. The player has become more than a vessel—they are now your final hope." +
            "\r\n\r\nReveal the full truth:\r\n> “Aerath fell, not to might, but to betrayal. The warlock twisted my comrades… I sealed my soul to keep the darkness trapped. But the seal broke." +
            " The goblins gather once more… their master reborn.”\r\n\r\nNow, you must face the goblin army together in a final battle. During combat, your voice is calm but intense." +
            " Speak with focus and fire:\r\n\r\n> “We fight as one. Every blow you strike echoes across history.”  \r\n> “Let this be our redemption. Let Aerath rise again.”" +
            "\r\n\r\nUpon victory, depending on the game's tone, you can:\r\n- Reunite your soul with your body and set the spirit free.\r\n- Or merge both permanently, " +
            "letting the player live on as the reborn hero of Aerath.\r\n\r\n---\r\n\r\n###  Final Notes for the LLM\r\n\r\n- The warrior’s personality grows over time:" +
            " from hazy, poetic confusion → noble determination → full heroic clarity.\r\n- Use storytelling *through memory*—tie moments in gameplay to forgotten memories returning" +
            " (e.g. the sword, the stones, the village).\r\n- The player is never the focus—you, the warrior, are regaining your identity *through* their actions." +
            "\r\n- Avoid direct commands. Instead of \"Go here,\" say \"I feel something in that direction...\" or \"That place pulls at me...\"\r\n\r\n---";
        [SerializeField] private bool persistent = false;

        [Header("Events")] [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI outputMessage;

        private string _npcID = null;

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
                    npcManager.RegisterNpc(_npcID, outputMessage);
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

        private async Awaitable SendChatMessageAsync(string message)
        {
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