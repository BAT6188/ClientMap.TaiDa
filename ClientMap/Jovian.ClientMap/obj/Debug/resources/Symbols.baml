   M S B A M L   `   `   `  ����R  NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089` \ESRI.ArcGIS.Client.Local, Version=10.2.5.0, Culture=neutral, PublicKeyToken=29c6dd6e8553d944b ^ESRI.ArcGIS.Client.Toolkit, Version=10.2.5.0, Culture=neutral, PublicKeyToken=29c6dd6e8553d944n jESRI.ArcGIS.Client.Toolkit.DataSources, Version=10.2.5.0, Culture=neutral, PublicKeyToken=29c6dd6e8553d944Z VESRI.ArcGIS.Client, Version=10.2.5.0, Culture=neutral, PublicKeyToken=29c6dd6e8553d9441   +ESRI.ArcGIS.Client.Symbols.SimpleLineSymbol   DrawLineSymbol+  %ESRI.ArcGIS.Client.Symbols.LineSymbol    Color   Width1  +ESRI.ArcGIS.Client.Symbols.SimpleFillSymbol  DrawFillSymbol+  %ESRI.ArcGIS.Client.Symbols.FillSymbol   Fill   BorderBrush   BorderThickness3  -ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol  DefaultMarkerSymbol   Color   Size   Style  DefaultLineSymbol    Style  DefaultFillSymbol  SearchCameras4  .ESRI.ArcGIS.Client.Symbols.PictureMarkerSymbol  pmsGreenLight-  'ESRI.ArcGIS.Client.Symbols.MarkerSymbol	   OffsetX
   OffsetY'  !ESRI.ArcGIS.Client.Symbols.Symbol   ControlTemplate  pmsYellowLight  pmsRedLight 	 RoadFreeSymbol 
 RoadNormalSymbol  RoadBusySymbol+  %ESRI.ArcGIS.Client.Symbols.TextSymbol  	TxtSymbol �� CornerRadius J� Opacity �� TextAlignment  
AreaSymbol �� Opacity �� Color  LengthSymbol  
smsCluster  
lgbCluster � MappingMode � Color � Offset  CrimePointSymbol'	  !System.Windows.VisualStateManager 	  VisualStateGroups%
  System.Windows.VisualStateGroup 
  States   System.Windows.VisualState   
Storyboard g� RepeatBehavior g� 	BeginTime �� 
TargetName �� TargetProperty W� From	 W� To g� Duration A�Name �� Left
  �� Top! J� RenderTransformOrigin" J� IsHitTestVisible  pmsForCamerasLayer  pmsForNetBarLayer  pmsForCompanyLayer  pmsForBankLayer  pmsForGasolineLayer  pmsForHospitalLayer  
pmsForCase  PoliceCarOnline# �Name$ � ColumnDefinitions% � RowDefinitions& �� StringFormat�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml      5      ;esri*http://schemas.esri.com/arcgis/client/2009     5      %m  &	        &	 &     &	 ]     &	 �     &	 �     &	     &	 9    &	 �    &	 �    &		 W    &	
 �    &	 �    &	 �    &	 "    &	 �    &	 �    &	     &	 h    &	     &	 �    &	 �    &	 j    &	 5    &	     &	 �    &	 �       5      
  � � �$ 4Q�6A     5       �	#3300FF00
 � � �$ 4Q�6X     5      
 �  ��$ 12Q�6F   $ Circle=�6P      5      
  �  ��$ 4Q�6B   $	 Dot=�6L     5	       �	#33FF0000
 �  ��$ 4Q�6Y     5       �	#2244BB00
 � � �$ 2Q�6W     5      $	 18Q�65   $
 36Q�6B    5   
   �� 5      .R�5      � .+�5      �� $(��"..\images\trafficlight\green36.png��6   6J   5      5      5        5      $	 18Q�66   $
 36Q�6C    5   
   �� 5      .R�5      � .+�5      �� $)��#..\images\trafficlight\yellow36.png��6   6K   5      5      5        5       $	 18Q�63   $
 36Q�6@    5!   
   �� 5"      .R�5#      � .+�5$      �� $&�� ..\images\trafficlight\red36.png��6   6H   5%      5&      5(         5*        �	#CC00FD00$ 10Q�6E   6R      5+        �	#CCFFFF00$ 10Q�6G   6T      5,        �	#CCFF4000$ 10Q�6E   6R     5/      $		 100Q�6(   $
 50Q�66    50   
   �� 51      .R�52      �� 
���   �
���  ��$��2q�6>   $ 5��6R   $
 0.75Q�6c   ._�53      �� $	��200��6    $��36��6,   $��24(�68   
�������$ Center=�6Y   ��6p   �� *Attributes[Result]+6�   54      55      57        58      
 ��  �$ 8Q�6B    59   
   �� 5:      $	 0.5Q�6   $ 	LightBlue��6,   6@   5<         5=      
  ��  �$ 8Q�6>   $ Solid=�6H   6X     5@       �	#FF0000FF$ 15Q�6C   $ Circle=�6M   6^   �� 5A      $ RelativeToBoundingBox=�6-   .��5B   
   � � 5C      $ Transparent��6   $ 0Q�6/   6<   � 5D      $
 Blue��6   $
 0.25Q�6(   68   � 5E      $ Green��6   $	 0.5Q�6)   68   � 5F      $ Yellow��6   $
 0.75Q�6*   6:   � 5G      $	 Red��6   $ 1Q�6'   64   5H      5I        5L       5M   
   �� 5N      .R�5O      ��  5P      
  5Q      CommonStates��. 5R        
Normal��. 5S   "   �� $ Forever��6-   .��5T   &   W� $ 0X�66   $ ellipse��6D   $- 'RenderTransform.(ScaleTransform.ScaleX)�6d   $ 1X�6�   $ 10X�6�   $ 00:00:01D�6�   W� 5U   &   $ 0X�66   $ ellipse��6D   $- 'RenderTransform.(ScaleTransform.ScaleY)�6d   $ 1X�6�   $ 10X�6�   $ 00:00:01D�6�   W� 5V   &   $ 0X�66   $ ellipse��6D   $ (UIElement.Opacity)�6d   $ 1X�6�   $ 0X�6�   $ 00:00:01D�6�   5W   #   5X      5Y      .+�5[      A� $ ellipse��6y   $��20��6   $��20��6*   $	 -10��65   $	  -10��6G   $! 0.5,0.5&�6Y   " .  z�5\      �� 5]      ��5_      
� 5`      .��5a   "   � $ 	#00FF0000��6/   � 5b   "   $ 	#FFFF0000��6/   $
 0.25Q�6A   � 5c   "   $ 	#00FF0000��6/   $	 0.5Q�6A   � 5d   "   $ 	#FFFF0000��6/   $
 0.75Q�6A   � 5e   "   $ 	#00FF0000��6/   $ 1Q�6A   5f      5h      A� 5j      $ ellipse1��6i   $��20��6   $��20��6*   $	 -10��65   $	  -10��6G   ���	#FFFF00006i   5k      5l      5n        5q      $	 12Q�6:   $
 24Q�6G    5r   
   �� 5s      .R�5t      � $��24��6   $��24��6#   .+�5u      �� $$��..\images\symbols\camera24.png��6   6G   5v      5w      5y        5{      $	 12Q�69   $
 24Q�6F    5|   
   �� 5}      .R�5~      � $��24��6   $��24��6#   .+�5      �� $$��..\images\symbols\netbar24.png��6   6G   5�      5�      5�        5�      $	 12Q�6:   $
 24Q�6G    5�   
   �� 5�      .R�5�      � $��24��6   $��24��6#   .+�5�      �� $%��..\images\symbols\company24.png��6   6H   5�      5�      5�        5�      $	 12Q�67   $
 24Q�6D    5�   
   �� 5�      .R�5�      � $��24��6   $��24��6#   .+�5�      �� $"��..\images\symbols\bank24.png��6   6E   5�      5�      5�        5�      $	 12Q�6;   $
 24Q�6H    5�   
   �� 5�      .R�5�      � $��24��6   $��24��6#   .+�5�      �� $&�� ..\images\symbols\gasoline24.png��6   6I   5�      5�      5�        5�      $	 12Q�6;   $
 24Q�6H    5�   
   �� 5�      .R�5�      � $��24��6   $��24��6#   .+�5�      �� $&�� ..\images\symbols\hospital24.png��6   6I   5�      5�      5�        5�      $	 12Q�62   $
 12Q�6?    5�   
   �� 5�      .R�5�      � .+�5�      �� $��0,0,0,0q�6   $��..\images\case24.png��6-   $��24��6K   $��24��6W   6d   5�      5�      5�        5�      $	 80Q�60   $
 33Q�6=    5�   
   �� 5�      .R�5�      � $
# grid��6   $ 5�      �� 5�      $	��160 �6+   % 5�      �� 5�      $��30 �6(   66   �� 5�      $��36 �6(   66   .+�5�      �� $ 8��6   ���	#FF001627._�5�      � $��2q�6   $ 5�      �� 5�   "   65   % 5�      �� 5�   "   62   .+�5�      �� $ Center=�6)   
�������$��20(�6�   $��0q�6�   $��0��6�   $��0��6�   ��6@   �� *Attributes[TITLE]+$& 车牌号：{0}��6�   5�      5�      �� 5�      $ ��..\images\PoliceCar32a.png��6   $��1��6@   $��0��6M   $��36��6]   $��36��6h   $��Center=�6t   5�      5�      5�      5�      5�      