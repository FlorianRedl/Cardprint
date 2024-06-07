# Cardprint
Cardprint is a versatile tool that simplifies the printing of smartcards with customizable layouts and dynamic data.
Whether for employee badges, membership cards, access control systems, or personalized gift cards, Cardprint provides an efficient way to create professional smartcards.



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
- Personalized gift cards


![Main View1](https://raw.githubusercontent.com/FlorianRedl/Cardprint/master/Screenshots/CardprintImg1.JPG)


## Getting Started:
1. Download the latest Cardprint Release.
2. Create or edit the example XML file to define your card layout.
3. Prepare your data in a CSV file (optional).
4. Use the Cardprint user interface to fill the layout fields and preview your cards.
5. Print your smartcards!


## XML Layout creation
### Supported Field Types:
- text
- image
  
### Supportet Formats:
- ISO/IEC 7810
- ID-0 (25.00 mm, 15.00 mm)
- ID-1 (85.60 mm, 53.98 mm)
- ID-2 (105.00 mm, 74.00 mm)
- ID-3 (125.00 mm, 88.00 mm)
- 
### example:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<layout>
    <format>ID-1</format>
    <image>
        <name>image1</name>
        <path>C:\temp\image1.jpg</path>
        <x>30</x>
        <y>5</y>
        <height>50</height>
    </image>
    <text>
        <name>Field 1</name>
        <x>10</x>
        <y>30</y>
        <size>20</size>
    </text>
    <text>
        <name>Field 2</name>
        <value>[date] / static testvalue</value>
        <x>8</x>
        <y>48</y>
        <size>12</size>
    </text>
</layout>
```

## Printer Settings
![Settings View](https://raw.githubusercontent.com/FlorianRedl/Cardprint/master/Screenshots/CardPrint_Settings.PNG)
