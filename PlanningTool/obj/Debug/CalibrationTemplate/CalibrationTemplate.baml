   M S B A M L   `   `   `  ����G  CPlanningTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:PlanningToolPlanningTool  5   	   &     PlanningTool.CalibrationTemplate   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:PlanningTool     5   	   -   5        $
��This��5   4     � Title$  CalibrationTemplate��5   	   $	��400��6%   $	��650��62    � 	Resources 5	      ,   &PlanningTool.BoolToVisibilityConverter   	boolToVis �� 
TargetType  GridBlockStyle �� Property �� Value �� Path �� RelativeSource �� AncestorType �� 	Converter	 �� ConverterParameter  GridEditStyle��%�  &	        &	      7   &	 �     7     5
   
   6*   �� 5   
   " ��6   .�5      ��  � ��$ Center=�62   �� 5       � y� 5      �� $ 
IsSelected� �� *FindAncestor+" ��8   $	 False��5      �� 5   
   " �6   .�5      ��  � ��$ Center=�62   �� 5       � y� 5      �� $ 
IsSelected� �� *FindAncestor+" ��8   $
	 True��5       5"      .��5#      �� .+�5$   
   �� -   5        
 ��Name$
 calibFileListView��5$      ��6.   ��  �� ElementName$
 This��$ CalibFileCollection� �� View 5&      �� 5'      .P�5)      ��  �� Width$	 300��6%   .��5*      �� 
 � Tag$ CalibrationFile��5+   #   $��CalibrationFile��5,   #    �� CellTemplate 5-      �� 5.      .R�5/   "   � .+�50   &   ]� .+�52   *   �� $
��Left=�64   $	��230��6p   $
��Left=�6|   $
��Wrap=�6�   ��6J   �� $ CalibrationFile�6�   �� 53   *   -   5        $��Right=�53   1   $��Right=�6H   $��Right=�6d   $��5q�6�   $��Browse��54   -   #��[ 6>   6e   55   '   56   #   57      59      �� 5;      $ 70��6%   .��5<      �� $ Currency��5=   #   $��Currency��5>   #    5?      �� 5@      .R�5A   "   � .+�5B   &   �� #��[ 5C      ��5B   0   �� $ Currency�5C      �� 5D   &   #��[ 5G       �� SelectedItem 5D   /   �� $ Currency���5E      �� $
 This��$ AvailableCurrencies�5G      5H   #   5I      5K      �� 5M      $	 200��6%   .��5N      �� $ AssumptionSet��5O   #   $��AssumptionSet��5P   #    5Q      �� 5R      .R�5S   "   � $��Stretch=�6'   .+�5T   &   �� ��60   �� $ AssumptionSet�5Z   #   5[      5]      5_      5a      �� 5b   
   .+�5c      �� -   5        $��Right=�5c      $��5q�61   $��CreateTemplate��5d      6+   5e      5f      5j      