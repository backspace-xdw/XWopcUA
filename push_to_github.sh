#!/bin/bash

# GitHub推送脚本
# 使用方法: ./push_to_github.sh <github-token>

if [ -z "$1" ]; then
    echo "请提供GitHub Personal Access Token作为参数"
    echo "使用方法: ./push_to_github.sh <your-github-token>"
    echo ""
    echo "如何获取Token:"
    echo "1. 访问 https://github.com/settings/tokens"
    echo "2. 点击 'Generate new token (classic)'"
    echo "3. 给Token命名，选择 'repo' 权限"
    echo "4. 生成并复制Token"
    exit 1
fi

TOKEN=$1
REPO_URL="https://backspace-xdw:${TOKEN}@github.com/backspace-xdw/XWopcUA.git"

echo "正在推送到GitHub..."
git remote set-url origin "$REPO_URL"
git push -u origin main

# 推送成功后，恢复不带token的URL
git remote set-url origin https://github.com/backspace-xdw/XWopcUA.git

echo "推送完成！"
echo "项目地址: https://github.com/backspace-xdw/XWopcUA"