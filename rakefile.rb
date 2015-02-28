require 'fileutils'
require 'albacore'
require "pathname"

PROJECT_NAME = "Storm.GoogleAnalytics"
REPORTING_PROJECT = "src/#{PROJECT_NAME}.Reporting/#{PROJECT_NAME}.Reporting.csproj"

BUILD_NUMBER_BASE = IO.read("VERSION")

WEB_OUTPUT_PATH = "/build"
CURRENT_PATH = File.expand_path(File.dirname('.'))
BUILD_PATH = "#{CURRENT_PATH}#{WEB_OUTPUT_PATH}"
DIST_PATH = "#{BUILD_PATH}/dist"
ZIP_PATH = "#{CURRENT_PATH}/release"

MSBUILD_PATH = File.join(ENV['WINDIR'], 'Microsoft.NET', 'Framework',  'v4.0.30319', 'MSBuild.exe')

task :default => ['debug']

namespace :albacore do
	msbuild :msbuild => [:prepare_directories, 'bundler:update'] do |msb|
			msb.targets [:clean, :build]
			msb.properties = {
					:configuration => BUILD_TARGET
			}
			msb.verbosity = "minimal"
			msb.solution = "#{PROJECT_NAME}.sln"
	end
	
	task :prepare_directories do
			FileUtils.rm_rf(BUILD_PATH) if File.directory? BUILD_PATH
			FileUtils.rm_rf(ZIP_PATH) if File.directory? ZIP_PATH
			FileUtils.mkdir(BUILD_PATH)
	end
	
	task :package do
		FileUtils.mkdir(ZIP_PATH)
		dirs = Dir["src/**/*.csproj"]
		dirs.each do |dir|
			puts dir
			p_dir = dir.gsub("\\") { "//" }

			system ".nuget/nuget.exe pack #{p_dir} -o #{ZIP_PATH} -Symbols -Properties Configuration=#{BUILD_TARGET}"
			
		end
	end
	
#	assemblyinfo :global_version do |asm|
#		commit_data = get_versions
#		commit = commit_data[0]
#		commit_date = commit_data[1]
#		tc_build_number = commit_data[2]
#		build_number = commit_data[3]
#		asm_version = commit_data[4]
#		
#		puts "Commit data: #{commit_data}"
#
#		puts "##teamcity[buildNumber '#{asm_version}']" unless tc_build_number.nil?
#		  
#		# Assembly file config
#		asm.product_name = PROJECT_NAME
#		asm.company_name = "Storm Id"
#		asm.description = "Git commit hash: #{commit} - #{commit_date}"
#		asm.version = build_number
#		asm.file_version = build_number
#		asm.custom_attributes :AssemblyInformationalVersion => "#{asm_version}", :ComVisibleAttribute => false
#		asm.copyright = "Copyright 2009-2014 StormId. All rights reserved."
#		asm.output_file = 'CommonAssemblyInfo.cs'
#		asm.namespaces "System", "System.Reflection", "System.Runtime.InteropServices", "System.Security"
#	end
end

namespace :bundler do
	desc "Bundle Update"
	task :update do
		system "bundle update"
	end
end

desc "Runs a Debug build, packages as -beta"
task :debug do
        BUILD_TARGET = :debug
        Rake::Task['albacore:msbuild'].invoke()
		Rake::Task['albacore:package'].invoke()
end
        
desc "Runs a Release build, does not package"
task :release do
        BUILD_TARGET = :release
        Rake::Task['albacore:msbuild'].invoke()
end

desc "Runs a Release build, packages"
task :publish => ['release', 'albacore:package']do
end

#def get_commit_hash_and_date
#	begin
#		commit = `git log -1 --pretty=format:%H`
#		git_date = `git log -1 --date=iso --pretty=format:%ad`
#		branch =  `git rev-parse --abbrev-ref HEAD`
#		commit_date = DateTime.parse( git_date ).strftime("%Y-%m-%d %H%M%S")
#	rescue
#		commit = ENV["BUILD_VCS_NUMBER"]
#	end
#
#	[commit, commit_date, branch]
#end

#def get_versions
#	commit_data = get_commit_hash_and_date
#	puts commit_data
#	commit = commit_data[0]
#	commit_date = commit_data[1]
#	tc_build_number = ENV["BUILD_NUMBER"].nil? ? Date.today.strftime('%y%j') : ENV["BUILD_NUMBER"]
#	build_number = BUILD_NUMBER_BASE + "." + tc_build_number
#	asm_version = BUILD_NUMBER_BASE + "." + tc_build_number
#	
#	if ( BUILD_TARGET == :debug )
#		puts "Branch name: #{ENV["BRANCHNAME"]}"
#		branch = commit_data[2].nil? ? ENV["BRANCHNAME"] : commit_data[2].gsub("\n","")
#		asm_version = asm_version + "-" + branch
#	end
#	
#	[commit, commit_date, tc_build_number, build_number, asm_version]
#end 