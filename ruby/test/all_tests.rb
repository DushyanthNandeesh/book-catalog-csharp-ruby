# frozen_string_literal: true

# Runs every test file. Usage (from the ruby folder): ruby test/all_tests.rb
Dir[File.join(__dir__, '*_test.rb')].sort.each { |file| require file }
