   M S B A M L   `   `   `  ���� %  !ZFreeGo.ChoicePhase.ControlCenter2    ,ZFreeGo.ChoicePhase.ControlCenter.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     \ XDynamicDataDisplay, Version=0.3.4703.0, Culture=neutral, PublicKeyToken=5b7d744a7263923f=d34http://research.microsoft.com/DynamicDataDisplay/1.0  -     � Title$&   选相合闸控制平台Platform��$��黑体)�$	��600��$	��800�� J� SnapsToDevicePixels . $	��480��$	��640�� � Icon$ pictrues\064.ico��.���  � RowDefinitions �� $��20 ��� $��* ��� $��20 �.+�o� $��0��.@�m� $��文件��m� $��编辑��m� $��视图��m� $��项目��m� $��帮助��� $��1��
����� � � ColumnDefinitions �� $	��150 ��� $��1* � �� $	��210 ��� $	��210 ��� $��* �.+�� -   $��gridSysState��$��0��$��0��$��3q�.+��� $��系统状态��
�������.���� -   $��systemStateGroup��.+���  �� Orientation$ 
Horizontal=�.+��� DSP通信状态:�� ����  �� Path$ CommonStateA����� $ ColorCommonStateA��� $ 
Horizontal=�.+��� 永磁通信状态:�� ���� $ CommonStateB��� $ 
Horizontal=�.+��� 同步开关状态:�� 
���  ������ $ SynSwitchStateA��� $ 
Horizontal=�.+��� 控制器状态：�� 
���  ������ $ ControllerStateA��� $ 
Horizontal=�.+��� 电容电压：�� 
���  ������ $ CapVoltageA��� $ 
Horizontal=�.+��� 主回路状态：�� 
���  ������ $ MainVCBStateA��� $ 
Horizontal=�.+��� A相电压：�� 
���  ������ $ MainCircleVoltageA��� $ 
Horizontal=�.+��� 检测系统状态：�� 
���  ������ $ CommonStateB��� -   $��tabUI��$��0��$��2��$��1��$��3q�.@��� $��DSP资源控制��.���� 
�������.+��� $��下位机选择��
����ح�.���� $ 
Horizontal=�$��0��.+��� $��3q�.��下位机选择：�� -   $��downMcuSelected��$��3q�$��60��	� -   $��fftRaio��$��30,3,3,3q�$��Center=� d� 	IsChecked$
 TrueY�.��	WithFFT	� -   $��zvdRaio��$��30,3,3,3q�$��Center=�.��	WithZVD�� $��控制面板��
����ح�.���� .+��� -   $ 
Horizontal=�.+��� $��3q�.��LED1:	� -	   $��Led1On��$��3q�$��Center=�.��点亮	� -
   $��Led1Off��$��3q�$��Center=�.��熄灭	� -   $��
Led1Toggle��$��3q�$��Center=�.��翻转�� -   $ 
Horizontal=�.+��� $��3q�.��LED2:	� -   $��Led2On��$��3q�$��Center=�.��点亮	� -   $��Led2Off��$��3q�$��Center=�.��熄灭	� -   $��
Led2Toggle��$��3q�$��Center=�.��翻转�� -   $ 
Horizontal=�.+��� $��3q�.��LED3:	� -   $��Led3On��$��3q�$��Center=�.��点亮	� -   $��Led3Off��$��3q�$��Center=�.��熄灭	� -   $��
Led3Toggle��$��3q�$��Center=�.��翻转�� -   $ 
Horizontal=�.+��� $��3q�.��LED4:	� -   $��Led4On��$��3q�$��Center=�.��点亮	� -   $��Led4Off��$��3q�$��Center=�.��熄灭	� -   $��
Led4Toggle��$��3q�$��Center=�.��翻转�� $ 
Horizontal=�.+��� $��3q�.��采样控制:�� -   $��3q�.��开始采样�� -   $��3q�.��停止采样�� -   $��30,3,3,3q�.��测试�� $ 
Horizontal=�.+��� $��3q�.��采样数据:�� -   $��3q�.��获取采样数据�� -   $��tipReciveTxt��$��3q�$��Center=�
�������$��楷体)�$��20(��� -   $��3q�.��数据另存为�� $ 
Horizontal=�.+��� $��3q�.��实时频率(Hz):�� -   $��3,3,3,3q�.��获取最新频率�� -   $��freqTxt��$��3q�$��Center=�
�������$��楷体)�$��20(�50.0�� $ 
Horizontal=�.+��� $��3q�.��实测周期:�� -    $��3,3,3,3q�.��获取计算周期�� -!   $��3,3,3,3q�.��获取初次时间差�� -"   $��3,3,3,3q�.��获取最终时间差�� -#   $��3,3,3,3q�.��获取相位�� $ 
Horizontal=�.+��� $��3q�.��时间差值(Us):�� -$   $��3,3,3,3q�$��零点侦测���� $ 
Horizontal=�.+��� $��3q�.��下位机控制:�� -%   $��3q�.��复位所选择的下位机� .+��� -&   $��3q�$��3q�.��设置同步信号�� -'   $��3q�$��3q�.��复位同步信号�� -(   $��3q�$��3q�.��测试触发等待信号�� $��通讯信息��.���  �� $��3* � �� $��94 ��� $��1* ��� $��94 ��� $��1* �.+��� $��	发送帧��
�������$��0��$��0��$��3q�.���  �� �� .+�� $��0��.+��� $��Center=�.��地址(Hex)：�� -)   $��deviceAddrTxt��$��30��$��3q�$��Center=�$��Center=�.��EA�� $��Center=�.��	功能：�� -*   $��
funCodeTxt��$��30��$��3q�$��Center=�$��Center=�.��31�� $��Center=�.��数据长度：�� -+   $��
dataLenTxt��$��30��$��3q�$��Center=�$��Center=�.��3�� -,   $��sendTest��$��20,3,3,3q�$��2q�$��1��.��发送]� $��1��.+��� $��Center=�.��	数据：�� --   $��dataTxt��$��3q�$��Center=�.��	F0 0 10�� $��发送数据(hex)��$��1��$��3q�
�������.���� -.   $��
sendTxtBox�� �� TextWrapping$
 Wrap=�.��.���� $��	接收帧��$��3,10,3,3q�
�������$��0��$��2��.���  �� �� .+�� $��3q�$��0��.+��� $��Center=�.��地址(Hex)：�� -/   $��deviceAddrReciveTxt��$��30��$��3q�$��Center=�$��Center=�.��2�� $��Center=�.��	功能：�� -0   $��funCodeReciveTxt��$��30��$��3q�$��Center=�$��Center=�.��1�� $��Center=�.��数据长度：�� -1   $��dataLenReciveTxt��$��30��$��3q�$��Center=�$��Center=�.��2]� $��1��.+��� $��Center=�.��	数据：�� -2   $��dataReciveTxt��$��3q�$��Center=�.��11�� $��接收数据(hex)��$��3��$��3q�
�������.���� -3   $��reciveTxtBox��$
 Wrap=�.��.���� $��同步控制��.���� $��同步控制��.���� .+��� $ 
Horizontal=�$��3q�.+��� $��3q�$��Center=�A相合闸相角：�� -4   $��setHezhaPhase��$��40��$��3q�$��Center=�.��0	� -5   $��jiaoDuRadio��$��Center=�$
 TrueY�.��角度	� -6   $��	huduRadio��$��Center=�.��弧度�� -7   $��30,3,3,3q�$��30��
���  ��.��合闸控制� $��5q� �� �� .+��� -8   $��cavas��$��0��$	��180��$	��240��
���   � . $��3q�.+�� -9   $��sinWave��
�������$��1��F� 
���������� .���� 	 �� 
StartPoint$
	 60,0&�
 �� EndPoint$
 60,180&��� $
	 0,90&�$
 240,90&��� -:   $��
lineCursor��
���  ��$��2��	 �� X1$ 60��	 �� Y1$	 150��	 �� X2$ 60��	 �� Y2$ 30���� -;   $��
cavasRight��$��1��$	��180��$	��240��
���   �$��3q�.+�F� 
���������� .��@�  @� RadiusX$ 50Q� @� RadiusY$ 50Q� @� Center$ 120,90&��� $	 120,0&�$
 120,180&��� $
	 0,90&�$
 240,90&��� -<   $��roundCursor��
���  ��$��2��$	 120��$ 90��$	 220��$ 90���� $ 
Horizontal=�$��3q�.+��� $��3q�$��Center=�实时合闸时间(us)：�� -=   $��realHezhaTime��$��40��$��3q�$��Center=�.��0�� ->   $��30,3,3,3q�$��30��
���  ��.��设置合闸时间�� $��控制器操作��.���� 
�������.+��� $��A相控制器��
����ح�.���� $ 
Horizontal=�$��0��.+��� $��3q�$��3q�$��50��.��LED1指示灯测试�� $��30,3,3,3q�$��3q�
���  ��.��
A相合闸�� $��30,3,3,3q�$��3q�
��� � �.��
A相分闸�� $��永磁控制器��.���� .+��� $��控制器选择��
����ح�.���� $ 
Horizontal=�$��0��.+��� $��3q�.��永磁控制器：�� -?   $��yongciSelectedComb��$��3q�$��60���� $��控制面板��
����ح�.���� .+��� -@   $ 
Horizontal=�.+��� .��
LED选择:�� -A   $��ledSelected��$��3q�$��60��	� -B   $��LedXOn��$��3q�$��Center=�.��点亮	� -C   $��LedXOff��$��3q�$��Center=�.��熄灭	� -D   $��
LedXToggle��$��3q�$��Center=�.��翻转�� $ 
Horizontal=�.+��� $��Center=�.��开关控制：�� -E   $��3q�$��3q�.��合闸控制�� -F   $��3q�$��3q�.��分闸控制�� $ 
Horizontal=�.+��� $��Center=�.��同步控制等待：�� -G   $��3q�$��3q�.��同步触发等待�� $ 
Horizontal=�.+��� $��Center=�.��合闸时间设置(ms)：�� -H   $��hezhaTimeTxt��$��30��$��Center=�.��50�� -I   $��3q�$��3q�.��设置合闸时间�� $ 
Horizontal=�.+��� $��Center=�.��分闸时间设置(ms)：�� -J   $��fenzhaTimeTxt��$��30��$��Center=�.��20�� -K   $��3q�$��3q�.��设置分闸时间�� $��循环操作控制��
����ح�.���� .+�� $��3q� �� $
��Auto ��� $��* ��� $
��Auto ��� $��* ��� $
��Auto ��� $��* ��� $��* � �� �� �� .+��� $��3q�$��Center=�$��0��$��0��.��次数�� -L   $��	TestCount��$��3q�$��Center=�$��1��$��0��.��2�� $��3q�$��Center=�$��2��$��0��.��时间C-t(s)-O�� -M   $��TestTime��$��3q�$��Center=�$��3��$��0��.��15�� $��3q�$��Center=�$��4��$��0��.��时间O-t(s)-C�� -N   $��	TestTimeB��$��3q�$��Center=�$��5��$��0��.��15�� -O   $��RunloopControl��$��3q�$��Center=�$��6��$��0��.��启动循环�� $��通信设置��.���  �� $	��110 ��� $	��110 �.+��� $��0��$��0��$��光纤通讯��
�������.���� .+�� -P   $��	commAGrid�� �� $��50 ��� $��80 ��� $��50 ��� $��80 ��� $��50 ��� $��80 � �� $��30 ��� $��30 �.+��� $��1q�$��0��$��0��$��Center=�.��
串口号:�� -Q   $��portName��$��1��$��0��$��30��$��22��$��Center=��� $��2q�$��1q�$��2��$��0��$��Center=�.��
波特率:�� -R   $��baudRate��$��3��$��0��$��30��$��22��$��Center=��� $��2q�$��1q�$��4��$��0��$��Center=�.��
数据位:�� -S   $��dataBits��$��5��$��0��$��30��$��22��$��Center=��� $��2q�$��1q�$��0��$��1��$��Center=�.��
校验位:�� -T   $��
portParity��$��1��$��1��$��30��$��22��$��Center=��� $��2q�$��1q�$��2��$��1��$��Center=�.��
停止位:�� -U   $��stopBits��$��3��$��1��$��30��$��22��$��Center=��� -V   $��openSerialPort��$��2q�$��3q�$��Center=�$��Center=�.��打开串口�� $��0��$��1��$��总线通信��
�������.���� .+�� -W   $��	commBGrid�� �� $��50 ��� $��80 ��� $��50 ��� $��80 ��� $��50 ��� $��80 � �� $��30 ��� $��30 �.+��� $��1q�$��0��$��0��$��Center=�.��
串口号:�� -X   $��	portNameB��$��1��$��0��$��30��$��22��$��Center=��� $��2q�$��1q�$��2��$��0��$��Center=�.��
波特率:�� -Y   $��	baudRateB��$��3��$��0��$��30��$��22��$��Center=��� $��2q�$��1q�$��4��$��0��$��Center=�.��
数据位:�� -Z   $��	dataBitsB��$��5��$��0��$��30��$��22��$��Center=��� $��2q�$��1q�$��0��$��1��$��Center=�.��
校验位:�� -[   $��portParityB��$��1��$��1��$��30��$��22��$��Center=��� $��2q�$��1q�$��2��$��1��$��Center=�.��
停止位:�� -\   $��	stopBitsB��$��3��$��1��$��30��$��22��$��Center=��� -]   $��openSerialPortB��$��2q�$��3q�$��Center=�$��Center=�.��打开串口� $��0��$��2��$��2��.+��� $��0��$��1��$��状态显示��
�������.���  �� $��* � �� $��4* ��� $��1* �.+��� -^   $��historyRecord��$��80��$��0���� $��0��$��1��.+��� -_   $��clearStateMessage��$��3q�$��3q�.��清除状态信息�� $��2��.@��� $��pictrues/067.ico��