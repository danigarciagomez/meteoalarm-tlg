   M S B A M L   `   `   `  ����I  Emeteoalarm-tlg, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:meteoalarm_tlgmeteoalarm_tlg  5   	       meteoalarm_tlg.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:meteoalarm_tlg     5   	     � Title$"  Avisos Telegram - MeteoAlarm��5   	   $	��170��6.   $	��800��6;   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��50px �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $	��300 �6   .+�5   
   �� -   5         ��Name$ 	BtnConfig��5      $��Configuración��6$   $
��Left=�6<   $��5,10,0,0q�6W   $	��Top=�6i   $��0��6�   $��1��6�   �� 5   
   -   5        $ BtnEjecManual��5      $��EjecutaManual��6(   $
��Left=�6@   $��
180,10,0,0q�6[   $	��Top=�6o   $��0��6�   $��1��6�   �� 5   
   -   5        $ 
BtnProceso��5      $
��Left=�6&   $��15,6,0,0q�6A   $	��Top=�6S   $��0��6k   $��0��6x   .��5      �� $��images/play_32x32.png��6   $��32��63   $��32��6?   5      �� 5   
   -   5         ��Name$ StatusImage��5      $��images/rojo_32x32.png��6%   $��32��6D   $��32��6P   $
��Left=�6[   $��60,7,0,0q�6v   $	��Top=�6�   $��0��6�   $��0��6�   �� 5   
   -   5        $��lblZonasAct��5      $��Zonas: ��6#   $
��Left=�65   $��5,1,0,0q�6P   $��1��6a   $��0��6n   $	��Top=�6~   �� 5   
   -   5        $��
lblNumEjec��5      $��Nº Ejecuciones: ��6"   $
��Left=�6=   $��5,1,0,0q�6X   $��1��6i   $��1��6v   $	��Top=�6�   �� 5    
   -   5        $��lblFecPrimeraEjec��5       $��Primera ejecución: ��6)   $
��Left=�6G   $��5,1,0,0q�6b   $��2��6s   $��0��6�   $	��Top=�6�   �� 5!   
   -   5        $��lblNumAvisosAct��5!      $$��Nº Avisos en XML (ult.ejec): ��6'   $
��Left=�6O   $��5,1,0,0q�6j   $��2��6{   $��1��6�   $	��Top=�6�   �� 5#   
   -	   5        $��lblFecUltEjec��5#      $��Última ejecución: ��6%   $
��Left=�6B   $��5,1,0,0q�6]   $��3��6n   $��0��6{   $	��Top=�6�   �� 5$   
   -
   5        $��lblNumAvisosSent��5$      $&�� Nº Avisos enviados (ult.ejec): ��6(   $
��Left=�6R   $��5,1,0,0q�6m   $��3��6~   $��1��6�   $	��Top=�6�   5&      5'      