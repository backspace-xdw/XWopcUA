# 推送到GitHub的详细指南

您的项目已经准备就绪，请选择以下任一方法完成推送：

## 方法1：使用Personal Access Token（推荐）

1. **获取GitHub Token**
   - 访问: https://github.com/settings/tokens
   - 点击 "Generate new token (classic)"
   - Token名称: 例如 "XWopcUA Push"
   - 选择权限: ✅ repo（全部）
   - 点击 "Generate token"
   - **立即复制生成的token**（只显示一次）

2. **使用推送脚本**
   ```bash
   cd /home/shenzheng/XWopcUA
   ./push_to_github.sh YOUR_GITHUB_TOKEN
   ```
   将 YOUR_GITHUB_TOKEN 替换为您复制的token

## 方法2：使用SSH密钥

1. **添加SSH公钥到GitHub**
   
   复制以下公钥内容：
   ```
   ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAILkkusLwD4QGrrfNk7mtHp2jNPLsmM+vJ9CqOyA4vK/H backspace-xdw@github.com
   ```

   然后：
   - 访问: https://github.com/settings/keys
   - 点击 "New SSH key"
   - Title: 例如 "XWopcUA Dev"
   - Key type: Authentication Key
   - Key: 粘贴上面的公钥内容
   - 点击 "Add SSH key"

2. **配置SSH并推送**
   ```bash
   cd /home/shenzheng/XWopcUA
   
   # 配置SSH
   echo "Host github.com
     HostName github.com
     User git
     IdentityFile ~/.ssh/id_ed25519_github" >> ~/.ssh/config
   
   # 更改远程URL为SSH
   git remote set-url origin git@github.com:backspace-xdw/XWopcUA.git
   
   # 推送
   git push -u origin main
   ```

## 方法3：手动输入凭据

```bash
cd /home/shenzheng/XWopcUA
git push -u origin main
```

当提示时输入：
- Username: backspace-xdw
- Password: 您的GitHub Personal Access Token（不是密码）

## 项目信息

- 本地路径: `/home/shenzheng/XWopcUA`
- 远程仓库: https://github.com/backspace-xdw/XWopcUA.git
- 分支: main
- 状态: 所有文件已提交，等待推送

## 推送后

成功推送后，您可以访问：
https://github.com/backspace-xdw/XWopcUA

查看您的OPC UA客户端项目！