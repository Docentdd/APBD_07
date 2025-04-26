
INSERT INTO Device (Id, Name, IsEnabled) VALUES
                                             ('SW-1', 'SWDevice1', 0),
                                             ('SW-2', 'SWDevice2', 1),
                                             ('P-1', 'PCDevice1', 1),
                                             ('P-2', 'PCDevice2', 0),
                                             ('E-1', 'EDevice1', 1),
                                             ('E-2', 'EDevice2', 0);


INSERT INTO Smartwatch (BatteryPercentage, DeviceId) VALUES
                                                         (75, 'SW-1'),
                                                         (24, 'SW-2');


INSERT INTO PersonalComputer (OperationSystem, DeviceId) VALUES
                                                             ('XP', 'P-1'),
                                                             ('Linux', 'P-2');


INSERT INTO Embedded (IpAddress, NetworkName, DeviceId) VALUES
                                                            ('192.168.0.101', 'MD Ltd. NetworkNam1', 'E-1'),
                                                            ('192.168.0.102', 'MD Ltd. NetworkName2', 'E-2');