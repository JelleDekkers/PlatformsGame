using UnityEngine;

public static class InputSystem {

	public static IInputSystem GetPlatformDependentInputSystem() {
        switch (Application.platform) {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                return new InputMobile();
            default:
                return new InputPC();
        }

    }
}
