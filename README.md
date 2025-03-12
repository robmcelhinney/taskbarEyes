# **Taskbar Eyes**

Taskbar Eyes is a quirky Windows application that brings a pair of animated eyes to your taskbar. The eyes track your mouse cursor, blink with a smooth animation when right-clicked, and offer toggles to change their size for a fun, interactive desktop experience.

---

## **Features**

-   **Dynamic Eye Tracking:**  
     The eyes follow your mouse cursor in real time.

-   **Blink Animation:**  
     Right-click an eye to trigger a smooth blink animation with Simpson’s yellow eyelids.

-   **Toggle Eye Size:**  
     Easily switch between the default and a doubled size view from the system tray.

-   **Taskbar Integration:**  
     The application positions itself over the taskbar for a unique look.

-   **System Tray Control:**  
     A tray icon allows you to toggle visibility, change eye size, and exit the application.

---

## **Demo**

https://github.com/user-attachments/assets/d2ef0f65-3277-484d-890c-eee607b97bd4

---

## **Installation**

### **Prerequisites**

**.NET 9.0 SDK or Later:**  
 Ensure you have the .NET 9.0 SDK installed. You can verify by running:

```sh
dotnet --list-sdks
```

-   **Windows Desktop Runtime:**  
     If you're building a WinForms app, make sure the required Windows Desktop Runtime is installed.

-

### **Cloning the Repository**

Clone the repository to your local machine:

```sh
git clone https://github.com/yourusername/TaskbarEyes.git

cd TaskbarEyes
```

---

## **Building and Publishing**

### **Build Locally**

To build the project in Debug mode:

```sh
dotnet build
```

### **Publish a Self-Contained Executable**

For a standalone Windows executable (x64):

```sh
dotnet publish -c Release -r win-x64 --self-contained true
```

The output executable will be located in:

`bin\Release\net9.0-windows\win-x64\publish\`

---

## **Usage**

1. **Run the Application:**  
   Launch the executable. The animated eyes will appear over your taskbar.

2. **Mouse Tracking:**  
   The eyes will continuously track your mouse cursor.

3. **Blink Animation:**  
   Right-click on either eye to trigger a blink animation with Simpsons yellow eyelids.

4. **Toggle Eye Size:**  
   Right-click the system tray icon and select "Double Eye Size" to toggle between the default and doubled eye sizes.

5. **Exit:**  
   Use the tray menu “Exit” option to close the application.

---

## **Customization**

-   **Eyelid Color:**  
     The blink overlays are drawn using Simpsons yellow (`RGB(255, 204, 51)`). Modify the color in the code if you prefer a different hue.

-   **Blink Speed:**  
     Adjust the `totalBlinkTicks` value in the source code to change the duration of the blink animation.

-   **Eye Size and Positioning:**  
     The default and double eye sizes/positions are defined in the source code. Feel free to tweak these values for a different visual effect.

---

## **Troubleshooting**

-   **Missing Runtimes:**  
     Ensure the appropriate Windows Desktop Runtime is installed if you encounter issues with WinForms apps.

-   **Target Framework Issues:**  
     Verify that your `<TargetFramework>` in `TaskbarEyes.csproj` is set to a valid value (e.g., `net9.0-windows`).

---

## **License**

This project is open source and available under the MIT License.

---

## **Contributing**

Feel free to fork the repository and submit pull requests for improvements or additional features. For major changes, please open an issue first to discuss what you would like to change.

---

Enjoy bringing a little personality to your taskbar with Taskbar Eyes!
