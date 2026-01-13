# MetacoreDevTemplate

### 해당 프로젝트는 Metalense 2 콘텐츠 제작에 필요한 패키지와 샘플 예제가 포함된 Unity 프로젝트입니다.
#### 본 프로젝트는 다음과 같은 구성 및 요구사항을 갖습니다.

- MRTK3(Mixed Reality Toolkit 3) 패키지가 포함되어 있으며 혼합현실 개발을 위한 기능을 제공합니다.

- Snapdragon Spaces 및 QCHT Interactions는 라이선스 정책에 따라 자동으로 포함되지 않으며, 필요 시 사용자가 직접 수동으로 프로젝트에 추가해야 합니다.

- 프로젝트는 Unity 2022.3.62f2 버전을 기반으로 개발되었습니다.

### 유니티 엔진 버전이 권장 환경보다 낮을 경우 다음과 같은 문제로 인해 앱 크래시가 발생할 수 있습니다.

- XrDisplaySubsystem 텍스처 파괴 버그(2021.3.54f1에서 수정) 렌더링이 끝나기 전에 텍스처를 파괴하려고 시도하던 문제가 존재했으며, FinishRendering을 사용하여 리소스가 해제될 때까지 대기하도록 수정되었습니다.

- 텍스처 업로드 시 메모리 누수 문제(2022.2.0f1에서 수정) 텍스처 업로드 과정에서 메모리가 정상적으로 해제되지 않아 누적되던 문제가 수정되었습니다.

#### 위와 같은 문제가 발생하는 경우 권장 환경을 참고하여 Unity 엔진 및 관련 라이브러리를 최신 버전으로 업데이트하시기 바랍니다.

## Snapdragon Spaces 및 QCHT Interations 패키기 적용 안내

1. [https://spaces.qualcomm.com/developer/](https://spaces.qualcomm.com/developer/) 에 접속합니다
2. 우측 상단 사람 아이콘을 클릭하고 **Create one**을 눌러 개발자 계정을 생성합니다.
    
    ![Image 01.png](docs/Image%2001.png)
    
    ![Image 02.png](docs/Image%2002.png)
    
3. 로그인 후 [https://spaces.qualcomm.com/developer/](https://spaces.qualcomm.com/developer/) 에서 AR Glasses 이미지 > Get Started 클릭
    
    ![Image 03.png](docs/Image%2003.png)
    
4. 좌측 하단 Unity > Download SDK 클릭
    
    ![Image 04.png](docs/Image%2004.png)
    
5. 다운로드 파일 확인
    
    ![Image 05.png](docs/Image%2005.png)
    

## 프로젝트 세팅

1. 제공된 레포지토리에 접근하여 그림과 같이 Download Zip을 눌러 프로젝트 설치  
    
    ![Image 06.png](docs/Image%2006.png)
    
2. MetacoreDevTemplate-main.zip 압축풀기
    
    ![Image 07.png](docs/Image%2007.png)
    
    ![Image 08.png](docs/Image%2008.png)
    
3. Snapdragon_Spaces_SDK_1_0_2_for_Unity.zip 압축풀기
    
    ![Image 09.png](docs/Image%2009.png)
    
4. Snapdragon_Spaces_SDK_1_0_2_for_Unity > Unity Package > qcht, spaces 파일 복사
    
    ![Image 10.png](docs/Image%2010.png)
    
5. MetacoreDevTemplate-main > metacoredevtemplate > Packages > 붙여 넣기
    
    ![Image 11.png](docs/Image%2011.png)
    

## 프로젝트 실행

1. Unity Hub > Projects > Add > Add project from dist 선택
    
    ![Image 12.png](docs/Image%2012.png)
    
    ![Image 13.png](docs/Image%2013.png)
    
2. Unity Hub > metacoredevtemplate 실행
    
    ![Image 14.png](docs/Image%2014.png)
    
    ![Image 15.png](docs/Image%2015.png)
    
3. Edit > Project Setting
    
    ![Image 16.png](docs/Image%2016.png)
    
4. XR Plug-in Management > Project Validation > Edit
    
    ![Image 17.png](docs/Image%2017.png)
    

## 프로젝트 빌드

1. File > Build Settings 클릭
    
    ![Image 18.png](docs/Image%2018.png)
    
2. Build 버튼 좌측 > ▼ > Clean Build…
    
    ![Image 19.png](docs/Image%2019.png)
    
3. 파일 이름 설정 > 저장
    
    ![Image 20.png](docs/Image%2020.png)
    
4. 빌드 완료

![Image 21.png](docs/Image%2021.png)

## 프로젝트 배포

1. scrcpy 설치 ([https://github.com/Genymobile/scrcpy/blob/master/doc/windows.md](https://github.com/Genymobile/scrcpy/blob/master/doc/windows.md))
    
    ![Image 22.png](docs/Image%2022.png)
    
2. metalense2 (Turn On 상태) ↔ PC USB 연결 > scrcpy.exe 실행
    
    ![Image 23.png](docs/Image%2023.png)
    
3. .apk 파일을 Drag & Drop
    
    ![Image 24.png](docs/Image%2024.png)
    
4. Performing Streamed Install > Success → 성공적으로 설치됨.
    
    ![Image 25.png](docs/Image%2025.png)
    

## 프로젝트 실행

1. 상단 빨간 영역을 마우크 좌클릭 상태로 > 아래로 드레그 
    
    ![Image 26.png](docs/Image%2026.png)
    
2. 빨간 영역을 한번 더 클릭 상태로 > 아래로 드레그
    
    ![Image 27.png](docs/Image%2027.png)
    
3. 빨간 영역의 설정 좌 클릭
    
    ![Image 28.png](docs/Image%2028.png)
    
4. 앱 메뉴 선택
    
    ![Image 29.png](docs/Image%2029.png)
    
5. 앱 모두 보기 선택
    
    ![Image 30.png](docs/Image%2030.png)
    
6. 앱 하단 > 설치된 앱 실행
    
    ![Image 31.png](docs/Image%2031.png)
    
7. 앱 > 열기 선택
    
    ![Image 32.png](docs/Image%2032.png)
    
8. 정상 동작 확인
    
    ![Image 33.png](docs/Image%2033.png)
    

## 프로젝트 제거

![Image 34.png](docs/Image%2034.png)


## 프로젝트 배포 (ADB)

    adb install [file_path]
    ex) adb install C:\MetacoreDevTemplate\Builds\builds.apk


## 프로젝트 실행 (ADB)

    adb shell monkey -p [package_name] -c android.intent.category.LAUNCHER 1
    ex) adb shell monkey -p com.metalense.MetacoreDevTemplate -c android.intent.category.LAUNCHER 1



## 프로젝트 제거 (ADB)

    adb uninstall [package_name]
    ex) adb uninstall com.metalense.MetacoreDevTemplate

