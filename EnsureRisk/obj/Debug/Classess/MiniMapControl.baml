   M S B A M L   `   `   `  ����  
EnsureRisk(   "EnsureRisk.Classess.MiniMapControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      -   5          >�Name$  controlRoot��5      $��50��5      $��50��5      ��5	      �� 5
   
   Style_1�� �� 
TargetType" >�6     �� Triggers 5      P� 5      	Trigger_1�� P� Property �@J�IsMouseCaptured P� Value . . �5      �� Setter_1�� �� Property �@�Cursor �� Value$ 	ScrollAll��6@   5      5      .��5      � -   5        $��contentGrid��5      contentGrid�� � ShowGridLines .   � ColumnDefinitions 5   
   �� 5      ColumnDefinition_1��$��0 �6:   �� 5      ColumnDefinition_2��$��* �6:   �� 5      ColumnDefinition_3��$��0 �6:   	 � RowDefinitions	 5   
   �� 5      RowDefinition_1��$��0 �64   �� 5      RowDefinition_2��$��* �64   �� 5      RowDefinition_3��$��0 �64   .+�5    
   �� -   5        $��Rectangle_1��5    )   Rectangle_1��$��1��6<   $��1��6L   
�������$��0��6g   ��5!      !� 5#      VisualBrush_1��
 j� Stretch$
 Uniform=�6w    !� Visual 64   ��  �� ElementName$ controlRoot�� �� Path$ MapSource.Content�6w   5%      �� 5(   
   -   5        $��lookupCanvas��5(   &   lookupCanvas��$��1��6:   $��1��6J   }�. .+�5)      �� -   5        $��lookupWindow��5)   v   lookupWindow��$��10��6-   $��10��68   
���  �����	#00ffd966$��2��6b    J� OpacityMask
 ��d� � Cursor$ SizeAll��6�   5*      5+      5,      