@echo off
cd /d %~dp0

ruby --version

call gem install bundler
call bundle install