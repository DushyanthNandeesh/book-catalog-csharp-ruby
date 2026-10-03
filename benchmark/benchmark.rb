# frozen_string_literal: true

# Performance benchmark (Ruby). Builds a large catalog and times the main operations.
# Run from the repository root:  ruby benchmark/benchmark.rb
require 'tmpdir'
require_relative '../ruby/catalog'
require_relative '../ruby/storage'

COUNT = 100_000 # books
RUNS = 20       # repeats for search and report timings

def now
  Process.clock_gettime(Process::CLOCK_MONOTONIC)
end

# Returns the average time in milliseconds for the block.
def average_ms(runs)
  start = now
  runs.times { yield }
  ((now - start) / runs) * 1000
end

# Same data generator as the C# benchmark, so both languages do identical work.
books = (1..COUNT).map do |i|
  Book.new(id: i, title: "Book #{i % 5000} Vol #{i}", author: "Author #{i % 800}",
           genre: "Genre #{i % 40}", year: 1900 + (i % 120))
end
catalog = Catalog.new(books)

search_ms = average_ms(RUNS) { catalog.search_by_title('vol 99') }
genre_ms = average_ms(RUNS) { catalog.group_by_genre }
author_ms = average_ms(RUNS) { catalog.group_by_author }

Dir.mktmpdir do |dir|
  path = File.join(dir, 'big.json')
  save_ms = average_ms(3) { Storage.save(path, catalog) }
  load_ms = average_ms(3) { Storage.load(path) }
  puts format('Ruby %-5s books=%d', RUBY_VERSION, COUNT)
  puts format('  search by title (avg of %d): %8.1f ms', RUNS, search_ms)
  puts format('  genre report    (avg of %d): %8.1f ms', RUNS, genre_ms)
  puts format('  author report   (avg of %d): %8.1f ms', RUNS, author_ms)
  puts format('  save JSON       (avg of 3):  %8.1f ms', save_ms)
  puts format('  load JSON       (avg of 3):  %8.1f ms', load_ms)
end
