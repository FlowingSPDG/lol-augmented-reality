# League of Legends Augumented Reality
## LoL Augumented Reality(Proof-of-Concept. WIP)

### works with Unity 2019.1.3f1
### Author : Shugo "FlowingSPDG" Kawamura

Working sample : https://www.youtube.com/watch?v=04IWwbfTpbA&feature=youtu.be  

NOTE : You need to enable ReplayAPI by editing `C:\Riot Games\League of Legends\Config\game.cfg`.  
`[General]`  
`EnableReplayApi=1`  

### Description(EN)
LoL's AR softwere by using replay-API.  
The softwere requests several informations to LoL client via HTTP,and apply those to Unity camera.  
The AR itself is impalamented with greenback,so you probably need to use chroma-key.

### 説明(JP)
LOLのReplay-APIを使用したARソフトウェアです。  
このソフトはLoLのクライアントにリクエストを要求し、取得した情報をUnityのカメラに適用しています。  
AR自体はグリーンバックでの運用を想定している為、OBSなどの外部ソフトを利用しクロマキー透過を行う事を推奨します。  
