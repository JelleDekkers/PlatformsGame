using UnityEngine;

public static class InputSystem {

	public static IInputSystem GetPlatformDependentInputSystem() {
        //switch (Application.platform) {
        //    case RuntimePlatform.Android:
        //        return new InputMobile();
        //    case RuntimePlatform.IPhonePlayer:
        //        return new InputMobile();
        //    default:
        //        return new InputPC();
        //}

        #if UNITY_ANDROID || UNITY_IOS
        return new InputMobile();
        #else
            return new InputPC();
        #endif
    }
}
