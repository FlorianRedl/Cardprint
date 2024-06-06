# Cardprint
Minimalist Tool to print SmartCards with predefined fields that can be dynamically filled with data.



- define Card Layouts via XML File
- View the Layouts with filled data
- Fill the Layout Fields with static or dynamic Data
- Load data from a csv Files
- Print a single or multiple Cards

### Field types :
- text
- image
  
### Supportet Formats:
- ID-1 (85.60 mm, 53.98 mm)
- ID-2 (105.00 mm, 74.00 mm)


### Main View
![Main View1](https://raw.githubusercontent.com/FlorianRedl/Cardprint/master/Screenshots/CardPrintNew1.PNG)

![Main View2](https://raw.githubusercontent.com/FlorianRedl/Cardprint/master/Screenshots/CardPrint3.PNG)

### Settings
![Settings View](https://raw.githubusercontent.com/FlorianRedl/Cardprint/master/Screenshots/CardPrint_Settings.PNG)

example layout file:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<layout>
    <format>ID-1</format>
    <image>
        <name>image1</name>
        <path>C:\temp\br.jpg</path>
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
        <name>Fiedl 2</name>
        <value>[date] / static testvalue</value>
        <x>8</x>
        <y>48</y>
        <size>12</size>
    </text>
</layout>
```
