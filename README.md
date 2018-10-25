# SimpleXAMLLocalizationHelper
Simple XAML Localization Helper for very limited situation.

모든 파일이 C:\Languages 폴더에 들어있고, 각 파일의 이름은 Korean.xaml, English.xaml, Chinese.xaml, Japanese.xaml이며 각 요소들은 x:Key 어트리뷰트를 가지고 있으며 값으로는 문자열만 존재하고 있는 상황에서만 정상적으로 문자열 값 변경에 한해서 사용 할 수 있는 프로그램입니다.
파일을 저장할 경우, 모든 \r\n과 \n을 &#xA;로 바꾸어 저장합니다.

이 프로그램은 XDocument를 이용한 XAML과 DataGridView의 Binding 예시용 프로그램입니다.
