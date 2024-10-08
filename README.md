# Cardprint
Cardprint is a versatile tool that simplifies the printing of smartcards with customizable layouts and dynamic data.
Whether for employee badges, membership cards, access control systems, or personalized gift cards, Cardprint provides an efficient way to create professional smartcards.

![Main View1](https://raw.githubusercontent.com/FlorianRedl/Cardprint/master/Screenshots/MainView1.png)

### Key Features
__Intuitive Layout Design:__ Easily define the layout of your cards using XML files. 

__Add and position various field types:__ Including text fields and image fields, to create your ideal card design.

__Dynamic Data Integration:__ Fill layout fields with static text or dynamic data from various sources, such as CSV files.

__Data Preview:__ View your layouts in real-time to ensure the design and data are correct before printing.

__Versatile Printing Options:__ Print individual cards or entire batches with just a few clicks.

### Potential Use Cases:
- Employee badges 
- access control cards
- customer loyalty cards
- Student ID cards
- library cards

# Getting Started:
1. Download the latest Cardprint Release.
2. Create or edit the example XML file to define your card layout.
3. Prepare your data in a CSV file (optional).
4. Use the Cardprint user interface to fill the layout fields and preview your cards.
5. Print your smartcards!

# XML Layout creation
### Supportet Formats:
- ISO/IEC 7810
  - ID-0 (25.00 mm, 15.00 mm)
  - ID-1 (85.60 mm, 53.98 mm)
  - ID-2 (105.00 mm, 74.00 mm)
  - ID-3 (125.00 mm, 88.00 mm)
### Supported Field Types:
<details>
  <summary>text</summary>
  <p>- required elements: x, y, name, size</p>
  <p>- optional elements: value</p>
</details>
<details>
  <summary>image</summary>
  <p>- required elements: x, y, path, width or heigth</p>
</details>

### positioning of the fields using coordinates
x => horizontal positioning starting from the left

y => vertical positioning starting from the top

### Static Field Values
These static field values are automatically inserted with each print, so it is possible, for example, to always print the current date.
- [date] returns the date in the following format "dd.MM.yyyy"
- [winUser]  returns the environment user name for the active Widows user

### example:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<layout>
    <format>ID-1</format>
    <image>
        <path>C:\temp\FLR Logo.png</path>
        <x>55</x>
        <y>8</y>
        <height>25</height>
    </image>
    <text>
        <name>Name</name>
        <x>8</x>
        <y>8</y>
        <size>20</size>
    </text>
    <text>
        <name>CID</name>
        <x>8</x>
        <y>15</y>
        <size>14</size>
    </text>
    <text>
        <name>Field 3</name>
        <value>[date] / static testvalue</value>
        <x>8</x>
        <y>48</y>
        <size>12</size>
    </text>
</layout>
```

# Settings
![Settings View](https://raw.githubusercontent.com/FlorianRedl/Cardprint/master/Screenshots/CardPrint_Settings.PNG)
