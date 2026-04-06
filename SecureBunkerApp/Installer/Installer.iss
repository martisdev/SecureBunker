#define AppVer GetFileVersion('..\bin\Release\net9.0-windows\SecureBunker.exe')
[Setup]
UsePreviousLanguage=no

AppName=Secure Bunker
AppVersion={#AppVer}
AppVerName=Secure Bunker {#AppVer}

DefaultGroupName=Secure Bunker

AppPublisher=MSC Soft
AppPublisherURL=https://msc-soft.com/
AllowNoIcons=yes

SetupIconFile="images\logo.ico"
WizardSmallImageFile="images\logo_55.bmp"
WizardImageFile="images\logo_vert_164.bmp"

DefaultDirName={pf}\SecureBunker

PrivilegesRequired=admin

ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Code]
var
FWCheckBox: TNewCheckBox;

function FW_Check(): Boolean;
begin
  if FWCheckBox.Checked then begin
    Result := true; 
    end
  else begin 
    Result := false; 
    end 
end;

// Check in regedit if FR Core 9 installed
// function hasDotNetCore(version: string) : boolean;
// var
// 	runtimes: TArrayOfString;
// 	I: Integer;
// 	versionCompare: Integer;
// 	registryKey: string;
// begin
// 	registryKey := 'SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.NETCore.App';
// 	if(not IsWin64) then
// 	   registryKey :=  'SOFTWARE\dotnet\Setup\InstalledVersions\x86\sharedfx\Microsoft.NETCore.App';
// 	   
// 	Log('[.NET] Look for version ' + version);
// 	   
// 	if not RegGetValueNames(HKLM, registryKey, runtimes) then
// 	begin
// 	  Log('[.NET] Issue getting runtimes from registry');
// 	  Result := False;
// 	  Exit;
// 	end;
// 	
//     for I := 0 to GetArrayLength(runtimes)-1 do
// 	begin
// 	  versionCompare := CompareVersion(runtimes[I], version);
// 	  Log(Format('[.NET] Compare: %s/%s = %d', [runtimes[I], version, versionCompare]));
// 	  if(not (versionCompare = -1)) then
// 	  begin
// 	    Log(Format('[.NET] Selecting %s', [runtimes[I]]));
// 	    Result := True;
// 	  	Exit;
// 	  end;
//     end;
// 	Log('[.NET] No compatible found');
// 	Result := False;
// end;

// It's quite possible that this function is already available to you in Inno Setup but in case it's not, here is the Compare Version code from stack overflow
// https://stackoverflow.com/questions/37825650/compare-version-strings-in-inno-setup
// function CompareVersion(V1, V2: string): Integer;
// var
//   P, N1, N2: Integer;
// begin
//   Result := 0;
//   while (Result = 0) and ((V1 <> '') or (V2 <> '')) do
//   begin
//     P := Pos('.', V1);
//     if P > 0 then
//     begin
//       N1 := StrToInt(Copy(V1, 1, P - 1));
//       Delete(V1, 1, P);
//     end
//       else
//     if V1 <> '' then
//     begin
//       N1 := StrToInt(V1);
//       V1 := '';
//     end
//       else
//     begin
//       N1 := 0;
//     end;

//     P := Pos('.', V2);
//     if P > 0 then
//     begin
//       N2 := StrToInt(Copy(V2, 1, P - 1));
//       Delete(V2, 1, P);
//     end
//       else
//     if V2 <> '' then
//     begin
//       N2 := StrToInt(V2);
//       V2 := '';
//     end
//       else
//     begin
//       N2 := 0;
//     end;

//     if N1 < N2 then Result := -1
//       else
//     if N1 > N2 then Result := 1;
//   end;
// end;

procedure CreateTheWizardPages;
var
  Page: TWizardPage;
  
  PWLabel,FWLabelDescrip: TNewStaticText;
  
begin
  Page := CreateCustomPage(wpWelcome, 'Previous installations', 'Auxiliar framework');

  FWCheckBox := TNewCheckBox.Create(Page);
  FWCheckBox.Width := Page.SurfaceWidth div 2;
  FWCheckBox.Height := ScaleY(17);
  FWCheckBox.Caption := 'Install Microsoft .NET Core 9';
  FWCheckBox.Checked := false;  
  FWCheckBox.Parent := Page.Surface;

  FWLabelDescrip := TNewStaticText.Create(Page);
  FWLabelDescrip.Top := FWCheckBox.Top + FWCheckBox.Height + ScaleY(8);
  FWLabelDescrip.Width := Page.SurfaceWidth div 2 - ScaleX(8);
  FWLabelDescrip.Caption := 'This application requires .Net Core 9';
  FWLabelDescrip.Parent := Page.Surface;  
    
end;

procedure InitializeWizard();
begin            
    CreateTheWizardPages;        
end;

[Tasks]
Name: desktopicon; Description: Create icon into desktop; GroupDescription: "Additional icons" ; 

[Files]
Source: "..\bin\Release\net9.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "dependency_app\windowsdesktop-runtime-9.0.14-win-x64.exe"; DestDir: "{tmp}"

[Icons]
Name: "{group}\Secure Bunker"; Filename: "{app}\SecureBunker.exe"; WorkingDir: "{app}" ;
Name: "{userdesktop}\Secure Bunker"; Filename: "{app}\SecureBunker.exe"; WorkingDir: "{app}"; Comment: "Save your secrets in a seave place"; Tasks: desktopicon

[Run]
; Installing .net in quiet, unattended mode. 
Filename: "{tmp}\windowsdesktop-runtime-9.0.14-win-x64.exe";Parameters: "/q"; WorkingDir:{tmp} ;Flags: runhidden; StatusMsg: Installing .Net Core 9; Check: FW_Check

[Dirs]
Name: "{app}\config"; Permissions: everyone-full
