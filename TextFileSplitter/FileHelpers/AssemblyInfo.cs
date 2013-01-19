using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

[assembly : AssemblyTitle("FileHelpers Library http://www.filehelpers.com")]

[assembly : ReflectionPermission(SecurityAction.RequestMinimum, ReflectionEmit = true, RestrictedMemberAccess = true)]
[assembly : SecurityPermission(SecurityAction.RequestMinimum, SerializationFormatter = true)]


[assembly : AssemblyDelaySign(false)]
[assembly : AssemblyKeyName("")]

[assembly: InternalsVisibleTo("FileHelpers.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010071634CEBD8DEBAE05841DF6D97B134B335B019F771467C700C64DE1A31EBE92784AA4EEE76C8E23D495622FFE910727BC2F24C41B7E46C61B88BF659B25034D58F685E533BC45F5CC26FB07AAAE85E86A931E97016DEA5D9D920E1C623433A45828BDAA5216F5FDE854673F26B6DEFAF7AA55706301CC94AF9B03BA3943288C5")]
[assembly: InternalsVisibleTo("FileHelpers.DataLink, PublicKey=002400000480000094000000060200000024000052534131000400000100010071634CEBD8DEBAE05841DF6D97B134B335B019F771467C700C64DE1A31EBE92784AA4EEE76C8E23D495622FFE910727BC2F24C41B7E46C61B88BF659B25034D58F685E533BC45F5CC26FB07AAAE85E86A931E97016DEA5D9D920E1C623433A45828BDAA5216F5FDE854673F26B6DEFAF7AA55706301CC94AF9B03BA3943288C5")]
[assembly: InternalsVisibleTo("FileHelpers.ExcelStorage, PublicKey=002400000480000094000000060200000024000052534131000400000100010071634CEBD8DEBAE05841DF6D97B134B335B019F771467C700C64DE1A31EBE92784AA4EEE76C8E23D495622FFE910727BC2F24C41B7E46C61B88BF659B25034D58F685E533BC45F5CC26FB07AAAE85E86A931E97016DEA5D9D920E1C623433A45828BDAA5216F5FDE854673F26B6DEFAF7AA55706301CC94AF9B03BA3943288C5")]