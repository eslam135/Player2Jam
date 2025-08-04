namespace player2_sdk
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ExampleFunctionHandler: MonoBehaviour
    {
        [SerializeField] GameObject chatHeader;
        public void HandleFunctionCall(FunctionCall functionCall)
        {
            if (functionCall.name == "Close")
            {
                closeChat();
            }
        }
        void closeChat()
        {
            chatHeader.SetActive(false);
        }

    }

}
